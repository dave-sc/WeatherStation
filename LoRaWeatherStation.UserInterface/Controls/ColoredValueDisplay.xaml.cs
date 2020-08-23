using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using LoRaWeatherStation.Utils.Drawing;
using LoRaWeatherStation.Utils.Reactive;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Controls
{
    public class ColoredValueDisplay : UserControl, IActivatableView
    {
        private TextBlock ValueText => this.FindControl<TextBlock>("ValueText");
        
        public ColoredValueDisplay()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.Value, x => x.ValueFormat, x => x.Unit)
                    .Select((value, format, unit) => $"{(Double.IsNaN(value) ? "??" : value.ToString(format))}{unit ?? ""}")
                    .BindTo(ValueText, x => x.Text)
                    .DisposeWith(disposables);
                
                this.WhenAnyValue(x => x.Value, x => x.ValueGradient)
                    .Select((value, stops) => new SolidColorBrush(stops.GetColorAtOffset(value)))
                    .BindTo(ValueText, x => x.Foreground)
                    .DisposeWith(disposables);
            });
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        /// <summary>
        /// Defines the <see cref="Value"/> property.
        /// </summary>
        public static readonly StyledProperty<double> ValueProperty =
            AvaloniaProperty.Register<ColoredValueDisplay, double>(nameof(Value));
        
        /// <summary>
        /// Gets or sets the displayed temperature
        /// </summary>
        public double Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        
        /// <summary>
        /// Defines the <see cref="ValueGradient"/> property.
        /// </summary>
        public static readonly StyledProperty<GradientStops> ValueGradientProperty =
            AvaloniaProperty.Register<ColoredValueDisplay, GradientStops>(nameof(ValueGradient), new GradientStops()
            {
                new GradientStop(Colors.Black, -100),
                new GradientStop(Colors.Black, 100),
            });
        
        /// <summary>
        /// Gets or sets the color gradient to use when applying a foreground color to the value display
        /// </summary>
        public GradientStops ValueGradient
        {
            get => GetValue(ValueGradientProperty);
            set => SetValue(ValueGradientProperty, value);
        }
        
        /// <summary>
        /// Defines the <see cref="ValueFormat"/> property.
        /// </summary>
        public static readonly StyledProperty<string> ValueFormatProperty =
            AvaloniaProperty.Register<ColoredValueDisplay, string>(nameof(ValueFormat), "0.0");
        
        /// <summary>
        /// Gets or sets the format string, that is used to format the value
        /// </summary>
        public string ValueFormat
        {
            get => GetValue(ValueFormatProperty);
            set => SetValue(ValueFormatProperty, value);
        }
        
        /// <summary>
        /// Defines the <see cref="Unit"/> property.
        /// </summary>
        public static readonly StyledProperty<string> UnitProperty =
            AvaloniaProperty.Register<ColoredValueDisplay, string>(nameof(Unit), "");
        
        /// <summary>
        /// Gets or sets the unit string that is appended to the displayed value
        /// </summary>
        public string Unit
        {
            get => GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }
    }
}