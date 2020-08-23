using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using WeatherLib;

namespace LoRaWeatherStation.UserInterface.Controls
{
    public class WeatherImage : UserControl, IActivatableView
    {
        private Image ImageControl => this.FindControl<Image>("Image");
        
        public WeatherImage()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.Weather)
                    .Select(x => new Classes(Enum.GetName(typeof(WeatherType), x)))
                    .BindTo(ImageControl, x => x.Classes)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        /// <summary>
        /// Defines the <see cref="Weather"/> property.
        /// </summary>
        public static readonly StyledProperty<WeatherType> WeatherProperty =
            AvaloniaProperty.Register<WeatherImage, WeatherType>(nameof(Weather));
        
        /// <summary>
        /// Gets or sets the type of weather for which the image should be displayed
        /// </summary>
        public WeatherType Weather
        {
            get => GetValue(WeatherProperty);
            set => SetValue(WeatherProperty, value);
        }
    }
}