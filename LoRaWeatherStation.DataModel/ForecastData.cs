using System;
using System.ComponentModel.DataAnnotations;
using WeatherLib;

namespace LoRaWeatherStation.DataModel
{
    public class ForecastData
    {
        [Key]
        public ForecastLocation Location { get; set; }
        
        public DateTime LoadTime { get; set; }
        
        [Key]
        public DateTime Time { get; set; }
        
        public double Temperature { get; set; }
        public double TemperatureError { get; set; }
        public double WindSpeed { get; set; }
        public double WindSpeedError { get; set; }
        public double Pressure { get; set; }
        public double PressureError { get; set; }
        public double CloudCover { get; set; }
        public WeatherType Weather { get; set; }
    }
}