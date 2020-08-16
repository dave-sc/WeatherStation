using System;
using LoRaWeatherStation.DataModel;
using Microsoft.EntityFrameworkCore;

namespace LoRaWeatherStation.Service
{
    public class WeatherStationContext : DbContext
    {
        public WeatherStationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Option> Config { get; set; }
        
        public DbSet<Sensor> Sensors { get; set; }
        
        public DbSet<SensorRecord> Records { get; set; }
        
        public DbSet<ForecastLocation> Locations { get; set; }
        
        public DbSet<ForecastData> Forecasts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}