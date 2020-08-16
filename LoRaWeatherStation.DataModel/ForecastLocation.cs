using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoRaWeatherStation.DataModel
{
    public class ForecastLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public double Longitude { get; set; }
        
        public double Latitude { get; set; }
    }
}