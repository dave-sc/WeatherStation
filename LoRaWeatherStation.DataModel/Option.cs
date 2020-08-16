using System.ComponentModel.DataAnnotations;

namespace LoRaWeatherStation.DataModel
{
    public class Option
    {
        [Key]
        public string Name { get; set; }
        
        public string Value { get; set; }
    }
}