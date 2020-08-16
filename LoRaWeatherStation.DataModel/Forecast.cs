using System;
using System.ComponentModel.DataAnnotations;

namespace LoRaWeatherStation.DataModel
{
    public class Forecast
    {
        [Key]
        public long Id { get; set; }
        
        public ForecastLocation Location { get; set; }
        
        public DateTime Time { get; set; }
    }
}