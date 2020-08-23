using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using LoRaWeatherStation.DataModel;
using LoRaWeatherStation.UserInterface.Infrastructure;
using WeatherLib;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class DashboardViewModel : ViewModelBase
    {
        public static IReadOnlyList<ForecastData> ExampleForecast => new[]
        {
            new ForecastData { Time = DateTime.Today.AddHours(00), Temperature = 19.0, Precipitation = 0.0, Weather = WeatherType.Clear }, 
            new ForecastData { Time = DateTime.Today.AddHours(01), Temperature = 17.7, Precipitation = 0.0, Weather = WeatherType.Clear }, 
            new ForecastData { Time = DateTime.Today.AddHours(02), Temperature = 16.5, Precipitation = 0.0, Weather = WeatherType.Clear }, 
            new ForecastData { Time = DateTime.Today.AddHours(03), Temperature = 16.2, Precipitation = 0.0, Weather = WeatherType.Clear }, 
            new ForecastData { Time = DateTime.Today.AddHours(04), Temperature = 15.8, Precipitation = 0.0, Weather = WeatherType.Clear }, 
            new ForecastData { Time = DateTime.Today.AddHours(05), Temperature = 16.6, Precipitation = 0.0, Weather = WeatherType.CloudyLight }, 
            new ForecastData { Time = DateTime.Today.AddHours(06), Temperature = 17.4, Precipitation = 0.0, Weather = WeatherType.CloudyLight }, 
            new ForecastData { Time = DateTime.Today.AddHours(07), Temperature = 19.1, Precipitation = 0.0, Weather = WeatherType.CloudyLight }, 
            new ForecastData { Time = DateTime.Today.AddHours(08), Temperature = 21.3, Precipitation = 0.0, Weather = WeatherType.CloudyLight }, 
            new ForecastData { Time = DateTime.Today.AddHours(09), Temperature = 22.5, Precipitation = 0.0, Weather = WeatherType.CloudyMedium }, 
            new ForecastData { Time = DateTime.Today.AddHours(10), Temperature = 23.9, Precipitation = 0.0, Weather = WeatherType.CloudyLight }, 
            new ForecastData { Time = DateTime.Today.AddHours(11), Temperature = 25.1, Precipitation = 0.0, Weather = WeatherType.CloudyLight }, 
            new ForecastData { Time = DateTime.Today.AddHours(12), Temperature = 26.5, Precipitation = 0.0, Weather = WeatherType.CloudyMedium }, 
            new ForecastData { Time = DateTime.Today.AddHours(13), Temperature = 27.7, Precipitation = 0.0, Weather = WeatherType.CloudyMedium }, 
            new ForecastData { Time = DateTime.Today.AddHours(14), Temperature = 29.1, Precipitation = 0.0, Weather = WeatherType.CloudyVery }, 
            new ForecastData { Time = DateTime.Today.AddHours(15), Temperature = 30.2, Precipitation = 0.0, Weather = WeatherType.CloudyVery }, 
            new ForecastData { Time = DateTime.Today.AddHours(16), Temperature = 30.6, Precipitation = 0.5, Weather = WeatherType.LightIntermittentDrizzle }, 
            new ForecastData { Time = DateTime.Today.AddHours(17), Temperature = 29.4, Precipitation = 0.7, Weather = WeatherType.LightRain }, 
            new ForecastData { Time = DateTime.Today.AddHours(18), Temperature = 28.8, Precipitation = 0.5, Weather = WeatherType.LightIntermittentDrizzle }, 
            new ForecastData { Time = DateTime.Today.AddHours(19), Temperature = 26.3, Precipitation = 1.1, Weather = WeatherType.Rain }, 
            new ForecastData { Time = DateTime.Today.AddHours(20), Temperature = 25.4, Precipitation = 1.5, Weather = WeatherType.Rain }, 
            new ForecastData { Time = DateTime.Today.AddHours(21), Temperature = 23.3, Precipitation = 0.6, Weather = WeatherType.LightRain }, 
            new ForecastData { Time = DateTime.Today.AddHours(22), Temperature = 21.9, Precipitation = 0.2, Weather = WeatherType.LightIntermittentDrizzle }, 
            new ForecastData { Time = DateTime.Today.AddHours(23), Temperature = 21.3, Precipitation = 0.0, Weather = WeatherType.CloudyVery }, 
            new ForecastData { Time = DateTime.Today.AddHours(24), Temperature = 20.7, Precipitation = 0.0, Weather = WeatherType.CloudyVery }, 
        };
        
        public DashboardViewModel(IScreen hostScreen)
                : base(hostScreen)
        {
            this.WhenActivated(disposables =>
            {
                _currentTime = Observable.Interval(TimeSpan.FromSeconds(0.5))
                    .Select(x => DateTime.Now)
                    .ObserveOn(SynchronizationContext.Current)
                    .ToProperty(this, x => x.CurrentTime)
                    .DisposeWith(disposables);                
            });
        }
        
        private ObservableAsPropertyHelper<DateTime> _currentTime;
        
        public DateTime CurrentTime => _currentTime.Value;
    }
}