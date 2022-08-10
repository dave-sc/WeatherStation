using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoRaWeatherStation.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace LoRaWeatherStation.Service.Controllers
{
    [ApiController]
    [Route("locations")]
    public class LocationsController : ControllerBase
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly WeatherStationContext _dbContext;
        private readonly ForecastLoader _forecastLoader;

        public LocationsController(IHostApplicationLifetime applicationLifetime, WeatherStationContext dbContext, ForecastLoader forecastLoader)
        {
            _applicationLifetime = applicationLifetime;
            _dbContext = dbContext;
            _forecastLoader = forecastLoader;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var locations = await _dbContext.Locations.ToArrayAsync();
            return Ok(locations);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] long id)
        {
            var (success, location) = await TryGetLocationById(id);
            if (!success)
                return NotFound();

            return Ok(location);
        }
        
        [HttpGet("{id}/weather")]
        public async Task<IActionResult> GetWeather([FromRoute] long id)
        {
            var (success, location) = await TryGetLocationById(id);
            if (!success)
                return NotFound();

            var forecast = await _dbContext.Forecasts
                .Where(f => f.Location == location && f.Time >= DateTime.UtcNow.AddHours(-1))
                .OrderBy(f => f.Time)
                .FirstOrDefaultAsync();

            if (forecast == null)
                return NotFound();
            
            return Ok(forecast);
        }
        
        [HttpGet("{id}/forecast")]
        public async Task<IActionResult> GetForecast([FromRoute] long id, [FromQuery] DateTime? day)
        {
            day = day?.Date ?? DateTime.UtcNow.Date;
            var tomorrow = day.Value.AddHours(24);
            
            var (success, location) = await TryGetLocationById(id);
            if (!success)
                return NotFound();

            var forecast = await _dbContext.Forecasts
                .Where(f => f.Location == location && f.Time >= day && f.Time < tomorrow)
                .OrderBy(f => f.Time)
                .ToArrayAsync();

            if (forecast == null)
                return NotFound();
            
            return Ok(forecast);
        }
        
        [HttpPost]
        public async Task<IActionResult> Post(IEnumerable<ForecastLocation> locations)
        {
            var newLocations = locations as IReadOnlyList<ForecastLocation> ?? locations.ToArray();
            var oldIds = _dbContext.Locations.Select(s => s.Id).ToArray();
            var newIds = newLocations.Select(s => s.Id).ToArray();
            
            _dbContext.RemoveRange(await _dbContext.Locations.Where(l => !newIds.Contains(l.Id)).ToArrayAsync());
            _dbContext.UpdateRange(newLocations.Where(l => oldIds.Contains(l.Id)));
            _dbContext.AddRange(newLocations.Where(l => !oldIds.Contains(l.Id)));
            await _dbContext.SaveChangesAsync();
            _ = _forecastLoader.UpdateForecastsAsync(_applicationLifetime.ApplicationStopping);

            return Ok(await _dbContext.Locations.ToListAsync());
        }
        
        private async Task<(bool, ForecastLocation)> TryGetLocationById(long id)
        {
            try
            {
                return (true, await _dbContext.Locations.SingleAsync(location => location.Id == id));
            }
            catch (InvalidOperationException)
            {
                return (false, null);
            }
        }
    }
}