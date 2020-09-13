using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class DashboardView : ReactiveUserControl<DashboardViewModel>
    {
        private SensorWidget MainSensorWidget => this.FindControl<SensorWidget>("MainSensorWidget");
        private SensorWidget SecondarySensorWidget => this.FindControl<SensorWidget>("SecondarySensorWidget");
        private SensorWidget TertiarySensorWidget => this.FindControl<SensorWidget>("TertiarySensorWidget");
        private LocationWidget LocationWidget => this.FindControl<LocationWidget>("LocationWidget");

        public DashboardView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.MainSensor, v => v.MainSensorWidget.Sensor)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.SecondarySensor, v => v.SecondarySensorWidget.Sensor)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.TertiarySensor, v => v.TertiarySensorWidget.Sensor)
                    .DisposeWith(disposables);
                
                this.OneWayBind(ViewModel, vm => vm.OutsideLocation, v => v.LocationWidget.Location)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.OutsideSensor, v => v.LocationWidget.Sensor)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}