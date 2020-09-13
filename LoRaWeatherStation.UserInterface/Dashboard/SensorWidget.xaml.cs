using System;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LoRaWeatherStation.DataModel;
using LoRaWeatherStation.UserInterface.Controls;
using LoRaWeatherStation.Utils.Reactive;
using ReactiveUI;
using Splat;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class SensorWidget : ReactiveUserControl<SensorWidgetViewModel>
    {
        private WidgetContainer Container => this.FindControl<WidgetContainer>("Container");
        private Grid ValueGrid => this.FindControl<Grid>("ValueGrid");
        private ColoredValueDisplay Primary => this.FindControl<ColoredValueDisplay>("Primary");
        private ColoredValueDisplay Secondary => this.FindControl<ColoredValueDisplay>("Secondary");
        
        public SensorWidget()
        {
            DataContext = Locator.Current.GetService<SensorWidgetViewModel>();
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(v => v.Sensor)
                    .BindTo(ViewModel, vm => vm.Sensor)
                    .DisposeWith(disposables);
                this.WhenAnyValue(v => v.PrimaryValueType)
                    .BindTo(ViewModel, vm => vm.PrimaryValueType)
                    .DisposeWith(disposables);
                this.WhenAnyValue(v => v.SecondaryValueType)
                    .BindTo(ViewModel, vm => vm.SecondaryValueType)
                    .DisposeWith(disposables);
                
                this.OneWayBind(ViewModel, vm => vm.Sensor.Name, v => v.Container.Header)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.PrimaryValueType, v => v.Primary.Classes, valueType => new Classes("primary", valueType.ToString().ToLower()))
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.PrimaryValue, v => v.Primary.Value)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.SecondaryValueType, v => v.Secondary.Classes, valueType => new Classes("secondary", valueType.ToString().ToLower()))
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.SecondaryValue, v => v.Secondary.Value)
                    .DisposeWith(disposables);

                this.WhenAnyValue(v => v.ValueGrid.Bounds, v => v.Secondary.IsVisible)
                    .Select((width, otherVisibility) => otherVisibility ? width.Width * 5d / 8d : width.Width)
                    .BindTo(this, v => v.Primary.Width);
                this.WhenAnyValue(v => v.ValueGrid.Bounds, v => v.Primary.IsVisible)
                    .Select((width, otherVisibility) => otherVisibility ? width.Width * 3d / 8d : width.Width)
                    .BindTo(this, v => v.Secondary.Width);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        /// <summary>
        /// Defines the <see cref="Sensor"/> property.
        /// </summary>
        public static readonly StyledProperty<Sensor> SensorProperty =
            AvaloniaProperty.Register<SensorWidget, Sensor>(nameof(Sensor));
        
        /// <summary>
        /// Gets or sets the sensor whose values should be displayed in this widget
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
            AvaloniaProperty.Register<SensorWidget, SensorValues>(nameof(PrimaryValueType), SensorValues.Temperature);
        
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
            AvaloniaProperty.Register<SensorWidget, SensorValues>(nameof(SecondaryValueType), SensorValues.None);
        
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