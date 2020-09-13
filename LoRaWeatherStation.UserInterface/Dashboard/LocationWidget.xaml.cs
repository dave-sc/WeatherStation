using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LoRaWeatherStation.DataModel;
using LoRaWeatherStation.UserInterface.Controls;
using ReactiveUI;
using Splat;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class LocationWidget : ReactiveUserControl<LocationWidgetViewModel>
    {
        private WidgetContainer Container => this.FindControl<WidgetContainer>("Container");
        private ColoredValueDisplay Primary => this.FindControl<ColoredValueDisplay>("Primary");
        private ColoredValueDisplay Secondary => this.FindControl<ColoredValueDisplay>("Secondary");
        private WeatherImage CurrentWeather => this.FindControl<WeatherImage>("Weather");
        private BasicForecastGraph ForecastGraph => this.FindControl<BasicForecastGraph>("Forecast");
        
        public LocationWidget()
        {
            DataContext = Locator.Current.GetService<LocationWidgetViewModel>();
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.Location)
                    .BindTo(ViewModel, vm => vm.Location)
                    .DisposeWith(disposables);
                this.WhenAnyValue(v => v.Sensor)
                    .BindTo(ViewModel, vm => vm.Sensor)
                    .DisposeWith(disposables);
                this.WhenAnyValue(v => v.PrimaryValueType)
                    .BindTo(ViewModel, vm => vm.PrimaryValueType)
                    .DisposeWith(disposables);
                this.WhenAnyValue(v => v.SecondaryValueType)
                    .BindTo(ViewModel, vm => vm.SecondaryValueType)
                    .DisposeWith(disposables);
                
                this.OneWayBind(ViewModel, vm => vm.Location.Name, v => v.Container.Header)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.WeatherForecast, v => v.ForecastGraph.Forecast)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.PrimaryValueType, v => v.Primary.Classes, valueType => new Classes("primary", valueType.ToString().ToLower()))
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.PrimaryValue, v => v.Primary.Value)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.SecondaryValueType, v => v.Secondary.Classes, valueType => new Classes("secondary", valueType.ToString().ToLower()))
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.SecondaryValue, v => v.Secondary.Value)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.CurrentWeather, v => v.CurrentWeather.Weather)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        /// <summary>
        /// Defines the <see cref="Location"/> property.
        /// </summary>
        public static readonly StyledProperty<ForecastLocation> LocationProperty =
            AvaloniaProperty.Register<LocationWidget, ForecastLocation>(nameof(Location));
        
        /// <summary>
        /// Gets or sets the location whose weather forecast should be displayed in this widget
        /// </summary>
        public ForecastLocation Location
        {
            get => GetValue(LocationProperty);
            set => SetValue(LocationProperty, value);
        }
        
        /// <summary>
        /// Defines the <see cref="Sensor"/> property.
        /// </summary>
        public static readonly StyledProperty<Sensor> SensorProperty =
            AvaloniaProperty.Register<LocationWidget, Sensor>(nameof(Sensor));
        
        /// <summary>
        /// Gets or sets the sensor whose values should be displayed in this widget instead of the forecast data for the current time
        /// </summary>
        public Sensor Sensor
        {
            get => GetValue(SensorProperty);
            set => SetValue(SensorProperty, value);
        }
        
        /// <summary>
        /// Defines the <see cref="PrimaryValueType"/> property.
        /// </summary>
        public static readonly StyledProperty<SensorValues> PrimaryValueTypeProperty =
            AvaloniaProperty.Register<LocationWidget, SensorValues>(nameof(PrimaryValueType), SensorValues.Temperature);
        
        /// <summary>
        /// Gets or sets the first sensor value that should be displayed in this widget
        /// </summary>
        public SensorValues PrimaryValueType
        {
            get => GetValue(PrimaryValueTypeProperty);
            set => SetValue(PrimaryValueTypeProperty, value);
        }
        
        /// <summary>
        /// Defines the <see cref="SecondaryValueType"/> property.
        /// </summary>
        public static readonly StyledProperty<SensorValues> SecondaryValueTypeProperty =
            AvaloniaProperty.Register<LocationWidget, SensorValues>(nameof(SecondaryValueType), SensorValues.Humidity);
        
        /// <summary>
        /// Gets or sets the second sensor value that should be displayed in this widget
        /// </summary>
        public SensorValues SecondaryValueType
        {
            get => GetValue(SecondaryValueTypeProperty);
            set => SetValue(SecondaryValueTypeProperty, value);
        }
    }
}