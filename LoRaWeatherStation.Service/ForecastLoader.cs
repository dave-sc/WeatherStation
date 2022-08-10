using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LoRaWeatherStation.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WeatherLib;

namespace LoRaWeatherStation.Service
{
    public class ForecastLoader : IHostedService, IDisposable
    {
        private readonly ILogger<ForecastLoader> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IWeatherDataProviderFactory _weatherDataProviderFactory;
        private readonly CancellationTokenSource _stopCancellationTokenSource;
        
        private Task _backgroundTask;

        public ForecastLoader(ILogger<ForecastLoader> logger, IServiceScopeFactory serviceScopeFactory, IWeatherDataProviderFactory weatherDataProviderFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _weatherDataProviderFactory = weatherDataProviderFactory;
            _stopCancellationTokenSource = new CancellationTokenSource();
            
            UpdateSchedule = new Queue<DateTime>(0);
        }

        public Queue<DateTime> UpdateSchedule { get; private set; }
        
        public DateTime? LastUpdate { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting forecast loader service");
            
            using var serviceScope = _serviceScopeFactory.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<WeatherStationContext>();
            
            var now = DateTime.UtcNow;
            var today = now.Date;
            var requiresUpdate = dbContext.Locations
                .Select(l => dbContext.Forecasts.Where(f => f.Location == l).OrderByDescending(f => f.Time).FirstOrDefault())
                .Select(f => f != null ? f.Time : DateTime.MinValue)
                .Any(t => t <= DateTime.UtcNow.AddHours(-1));
            UpdateSchedule = new Queue<DateTime>(Enumerable.Range(requiresUpdate ? now.Hour : now.Hour + 1, 24).Select(h => today.AddHours(h)));
            
            _backgroundTask = BeginBackgroundLoop(_stopCancellationTokenSource.Token);

            return Task.CompletedTask;
        }

        private async Task BeginBackgroundLoop(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                
                while (!cancellationToken.IsCancellationRequested)
                {
                    bool isUpdateBecauseOfSchedule;
                    if (UpdateSchedule.Count > 0 && DateTime.UtcNow >= UpdateSchedule.Peek())
                    {
                        isUpdateBecauseOfSchedule = true;
                    }
                    else if (LastUpdate == null)
                    {
                        isUpdateBecauseOfSchedule = false;
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
                        continue;
                    }

                    _logger.LogInformation("Starting periodic forecast update (is scheduled: {flag})", isUpdateBecauseOfSchedule);
                    try
                    {
                        await UpdateForecastsAsync(cancellationToken);

                        if (isUpdateBecauseOfSchedule)
                            UpdateSchedule.Enqueue(UpdateSchedule.Dequeue().AddDays(1));
                
                        _logger.LogInformation("Periodic forecast update completed. (Next update will occur on {timestamp})", UpdateSchedule.Peek());
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Periodic forecast update failed, will retry in 15 minutes.");
                        await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Periodic forecast update has been canceled");
            }
        }

        public async Task UpdateForecastsAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            using var serviceScope = _serviceScopeFactory.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<WeatherStationContext>();

            cancellationToken.ThrowIfCancellationRequested();

            var locations = await dbContext.Locations.AsNoTracking().ToArrayAsync(cancellationToken);
            
            _logger.LogInformation("Updating forecast data for {num} locations", locations.Length);
            await Task.WhenAll(locations.Select(l => UpdateForecastAsync(l, cancellationToken)));
            
            LastUpdate = DateTime.UtcNow;
            _logger.LogInformation("Update of forecast data completed successfully on {timestamp}", LastUpdate);
        }

        private async Task UpdateForecastAsync(ForecastLocation location, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            using var serviceScope = _serviceScopeFactory.CreateScope();
            using var dbContext = serviceScope.ServiceProvider.GetRequiredService<WeatherStationContext>();
            dbContext.Attach(location); // TODO: Why is this necessary?
            
            var weatherDataProvider = _weatherDataProviderFactory.GetProvider(new WeatherLocation(location.Name, location.Longitude, location.Latitude));
            
            _logger.LogInformation("Fetching forecast for location '{location}'", location.Name);
            cancellationToken.ThrowIfCancellationRequested();
            var dataPoints = await weatherDataProvider.GetForecastAsync();
            cancellationToken.ThrowIfCancellationRequested();
            
            var forecast = dataPoints.Select(f => new ForecastData()
            {
                Location = location,
                LoadTime = DateTime.UtcNow,
                Time = f.Time.Kind == DateTimeKind.Utc ? f.Time : throw new InvalidOperationException($"Excepted DateTimeKind.Utc for ForecastDataPoint, got '{f.Time.Kind}'. Timestamp: {f.Time}"),
                Temperature = f.Temperature,
                TemperatureError = f.TemperatureError,
                Precipitation = f.Precipitation,
                PrecipitationProbability = f.PrecipitationProbability,
                WindSpeed = f.WindSpeed,
                WindSpeedError = f.WindSpeedError,
                WindDirection = f.WindDirection,
                Pressure = f.Pressure,
                CloudCover = f.CloudCover,
                Weather = f.Weather,
            }).ToArray();
            
            cancellationToken.ThrowIfCancellationRequested();

            var forecastStartTime = forecast.Min(f => f.Time);
            var outdatedForecast = await dbContext.Forecasts
                .Where(f => f.Location == location)
                .Where(f => f.Time >= forecastStartTime)
                .ToArrayAsync(cancellationToken);
            dbContext.RemoveRange(outdatedForecast);
            await dbContext.Forecasts.AddRangeAsync(forecast);

            cancellationToken.ThrowIfCancellationRequested();
            await dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Fetched {new} new and deleted {old} old forecast data points for location '{location}'", forecast.Length, outdatedForecast.Length, location.Name);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping forecast loader service");
            
            _stopCancellationTokenSource.Cancel();
            await _backgroundTask;
        }

        public void Dispose()
        {
            _stopCancellationTokenSource.Cancel();
        }
    }
}