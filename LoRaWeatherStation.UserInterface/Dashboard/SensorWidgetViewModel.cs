using System;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LoRaWeatherStation.Client;
using LoRaWeatherStation.DataModel;
using LoRaWeatherStation.UserInterface.Infrastructure;
using LoRaWeatherStation.Utils.Reactive;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class SensorWidgetViewModel : ViewModelBase
    {
        private readonly WeatherStationClient _apiClient;

        public SensorWidgetViewModel(WeatherStationClient apiClient)
        {
            _apiClient = apiClient;
            
            this.WhenActivated(disposables =>
            {
                var sensorData = this.WhenAnyValue(vm => vm.Sensor)
                    .RepeatLastWhen(TimeSpan.FromMinutes(1))
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .Select(GetCurrentSensorValues);

                var primaryValueType = this.WhenAnyValue(vm => vm.PrimaryValueType);
                var secondaryValueType = this.WhenAnyValue(vm => vm.SecondaryValueType);
                
                _primaryValue = sensorData.CombineLatest(primaryValueType)
                    .Select((record, value) => record?.GetValue(value) ?? Double.NaN)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToProperty(this, vm => vm.PrimaryValue, Double.NaN)
                    .DisposeWith(disposables);
                
                _secondaryValue = sensorData.CombineLatest(secondaryValueType)
                    .Select((record, value) => record?.GetValue(value) ?? Double.NaN)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToProperty(this, vm => vm.SecondaryValue, Double.NaN)
                    .DisposeWith(disposables);
            });
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

        private ObservableAsPropertyHelper<double> _primaryValue;
        public double PrimaryValue => _primaryValue.Value;

        private ObservableAsPropertyHelper<double> _secondaryValue;
        public double SecondaryValue => _secondaryValue.Value;
        
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
    }
}