using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoRaWeatherStation.DataModel
{
    public class Sensor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        
        public string Name { get; set; }
        
        public SensorValues SupportedValues { get; set; }

        public override string ToString()
        {
            return $"{{{nameof(Id)}: {Id}, {nameof(Name)}: {Name}}}";
        }
    }
}