using LoRaWeatherStation.DataModel;

namespace LoRaWeatherStation.UserInterface.Configuration
{
    public class DashboardOptions
    {
        public const string SectionName = "Dashboard";
        
        public Sensor MainSensor { get; set; }
        public Sensor SecondarySensor { get; set; }
        public Sensor TertiarySensor { get; set; }
        public Sensor OutsideSensor { get; set; }
        public ForecastLocation OutsideLocation { get; set; }
    }
}