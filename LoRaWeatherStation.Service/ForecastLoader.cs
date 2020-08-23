using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GeoTimeZone;
using LoRaWeatherStation.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TimeZoneConverter;
using WeatherLib;

namespace LoRaWeatherStation.Service
{
    public class ForecastLoader : IHostedService, IDisposable
    {
        private readonly ILogger<ForecastLoader> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IWeatherDataProviderFactory _weatherDataProviderFactory;
        private readonly CancellationTokenSource _stopCancellationTokenSource;
        
        private Timer _updateTimer;

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
            //var requiresUpdate = dbContext.Forecasts
            //    .GroupBy(f => f.Location).ToArray()
            //    .Select(g => g.Max(f => f.Time))
            //    .Any(t => t <= DateTime.UtcNow.AddHours(-1));
            UpdateSchedule = new Queue<DateTime>(Enumerable.Range(requiresUpdate ? now.Hour : now.Hour + 1, 24).Select(h => today.AddHours(h)));
            _updateTimer = new Timer(OnUpdateTimerTick, null, TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        private void OnUpdateTimerTick(object? state)
        {
            bool isUpdateBecauseOfSchedule;
            if (UpdateSchedule.Count > 0 && DateTime.UtcNow >= UpdateSchedule.Peek())
                isUpdateBecauseOfSchedule = true;
            else if (LastUpdate == null)
                isUpdateBecauseOfSchedule = false;
            else
                return;

            _logger.LogInformation("Starting periodic forecast update (is scheduled: {flag})", isUpdateBecauseOfSchedule);
            try
            {
                UpdateForecastsAsync(_stopCancellationTokenSource.Token).GetAwaiter().GetResult();

                if (isUpdateBecauseOfSchedule)
                    UpdateSchedule.Enqueue(UpdateSchedule.Dequeue().AddDays(1));
                
                _logger.LogInformation("Periodic forecast update completed. (Next update will occur on {timestamp})", UpdateSchedule.Peek());
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
            
            var timeZoneName = TimeZoneLookup.GetTimeZone(location.Latitude, location.Longitude).Result;
            var locationTimeZone = TZConvert.GetTimeZoneInfo(timeZoneName);
            var forecast = dataPoints.Select(f => new ForecastData()
            {
                Location = location,
                LoadTime = DateTime.UtcNow, 
                Time = f.Time.Kind != DateTimeKind.Unspecified ? TimeZoneInfo.ConvertTimeToUtc(f.Time) : TimeZoneInfo.ConvertTime(f.Time, locationTimeZone, TimeZoneInfo.Utc),
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping forecast loader service");
            
            _stopCancellationTokenSource.Cancel();
            _updateTimer?.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _stopCancellationTokenSource.Cancel();
            _updateTimer?.Dispose();
        }
    }
}