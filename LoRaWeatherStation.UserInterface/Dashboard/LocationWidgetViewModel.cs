using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LoRaWeatherStation.Client;
using LoRaWeatherStation.DataModel;
using LoRaWeatherStation.UserInterface.Infrastructure;
using LoRaWeatherStation.Utils.Reactive;
using ReactiveUI;
using WeatherLib;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class LocationWidgetViewModel : ViewModelBase
    {
        private readonly WeatherStationClient _apiClient;
        
        public LocationWidgetViewModel(WeatherStationClient apiClient)
        {
            _apiClient = apiClient;
            
            this.WhenActivated(disposables =>
            {
                var location = this.WhenAnyValue(vm => vm.Location);
                var sensor = this.WhenAnyValue(vm => vm.Sensor);
                
                var forecastData = location
                    .RepeatLastWhen(TimeSpan.FromMinutes(15))
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Select(GetWeatherForecast);
                var weatherData = location
                    .RepeatLastWhen(TimeSpan.FromMinutes(1))
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Select(GetCurrentWeather);
                var sensorData = sensor
                    .RepeatLastWhen(TimeSpan.FromMinutes(1))
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Select(GetCurrentSensorValues);
                var fallbackData = weatherData
                    .Select(ExtractFallbackSensorValuesFromWeatherData);

                var currentData = sensor.CombineLatest(sensorData, fallbackData)
                    .Select((sensor, sensorData, fallbackData) => sensor != null ? sensorData : fallbackData);
                
                var primaryValueType = this.WhenAnyValue(vm => vm.PrimaryValueType);
                var secondaryValueType = this.WhenAnyValue(vm => vm.SecondaryValueType);
                    
                _weatherForecast = forecastData
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToProperty(this, x => x.WeatherForecast)
                    .DisposeWith(disposables);
                
                _primaryValue = currentData.CombineLatest(primaryValueType)
                    .Select((record, value) => record?.GetValue(value) ?? Double.NaN)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToProperty(this, vm => vm.PrimaryValue, Double.NaN)
                    .DisposeWith(disposables);
                
                _secondaryValue = currentData.CombineLatest(secondaryValueType)
                    .Select((record, value) => record?.GetValue(value) ?? Double.NaN)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToProperty(this, vm => vm.SecondaryValue, Double.NaN)
                    .DisposeWith(disposables);

                _currentWeather = weatherData
                    .Select(weather => weather?.Weather ?? WeatherType.None)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToProperty(this, vm => vm.CurrentWeather, WeatherType.None)
                    .DisposeWith(disposables);
            });
        }

        private ForecastLocation _location;
        
        public ForecastLocation Location
        {
            get => _location;
            set => this.RaiseAndSetIfChanged(ref _location, value);
        }

        private Sensor _sensor;
        
        public Sensor Sensor
        {
            get => _sensor;
            set => this.RaiseAndSetIfChanged(ref _sensor, value);
        }

        private SensorValues _primaryValueType;

        public SensorValues PrimaryValueType
        {
            get => _primaryValueType;
            set => this.RaiseAndSetIfChanged(ref _primaryValueType, value);
        }

        private SensorValues _secondaryValueType;

        public SensorValues SecondaryValueType
        {
            get => _secondaryValueType;
            set => this.RaiseAndSetIfChanged(ref _secondaryValueType, value);
        }

        private ObservableAsPropertyHelper<IReadOnlyList<ForecastData>> _weatherForecast;

        public IReadOnlyList<ForecastData> WeatherForecast => _weatherForecast.Value;

        private ObservableAsPropertyHelper<double> _primaryValue;
        public double PrimaryValue => _primaryValue.Value;

        private ObservableAsPropertyHelper<double> _secondaryValue;
        public double SecondaryValue => _secondaryValue.Value;

        private ObservableAsPropertyHelper<WeatherType> _currentWeather;
        public WeatherType CurrentWeather => _currentWeather.Value;
        
        private async Task<ForecastData[]> GetWeatherForecast(ForecastLocation location)
        {
            if (location == null)
                return new ForecastData[0];
            
            try
            {
                return await _apiClient.GetWeatherForecast(location);
            }
            catch (OperationCanceledException)
            {
                return new ForecastData[0];
            }  
            catch (HttpRequestException)
            {
                return new ForecastData[0];
            }            
        }
        
        private async Task<ForecastData> GetCurrentWeather(ForecastLocation location)
        {
            if (location == null)
                return null;
            
            try
            {
                return await _apiClient.GetCurrentWeather(location);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }            
        }
        
        private async Task<SensorRecord> GetCurrentSensorValues(Sensor sensor)
        {
            if (sensor == null)
                return null;
            
            try
            {
                return await _apiClient.GetCurrentSensorValues(sensor);
            }
            catch (OperationCanceledException)
            {
                return null;
            }    
            catch (HttpRequestException)
            {
                return null;
            }            
        }

        private SensorRecord ExtractFallbackSensorValuesFromWeatherData(ForecastData forecastData)
        {
            if (forecastData == null)
                return null;
            
            return new SensorRecord()
            {
                Temperature = (float) forecastData.Temperature,
                Humidity = float.NaN,
                Pressure = (float) forecastData.Pressure,
                WindSpeed = (float) forecastData.WindSpeed,
            };
        }
    }
}