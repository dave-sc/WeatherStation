using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoRaWeatherStation.DataModel;
using LoRaWeatherStation.UserInterface.Configuration;
using LoRaWeatherStation.UserInterface.Infrastructure;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class DashboardViewModel : PageViewModel
    {
        private readonly DashboardOptions _options;

        public DashboardViewModel(IScreen hostScreen, DashboardOptions options)
            : base(hostScreen)
        {
            _options = options;
        }

        public Sensor MainSensor => _options.MainSensor;
        public Sensor SecondarySensor => _options.SecondarySensor;
        public Sensor TertiarySensor => _options.TertiarySensor;
        public Sensor OutsideSensor => _options.OutsideSensor;
        public ForecastLocation OutsideLocation => _options.OutsideLocation;
    }
}