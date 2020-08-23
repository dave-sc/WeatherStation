using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using DynamicData.Binding;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Controls
{
    public class ClockControl : UserControl, IActivatableView
    {
        private TextBlock TimeText => this.FindControl<TextBlock>("TimeText");
        private TextBlock DateText => this.FindControl<TextBlock>("DateText");
        private Grid SecondsRing => this.FindControl<Grid>("SecondsRing");
        
        public static IValueConverter SecondsToAngleConverter = new FuncValueConverter<int, double>(s => (360d*s)/60d);
        
        public ClockControl()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                var time = this.WhenValueChanged(c => c.Time);

                time.Select(t => t.ToString("HH:mm"))
                    .BindTo(TimeText, x => x.Text)
                    .DisposeWith(disposables);
                time.Select(t => t.ToString("ddd. dd. MMM"))
                    .BindTo(DateText, x => x.Text)
                    .DisposeWith(disposables);
                time.Select(t => new RotateTransform((360d * t.Second) / 60d))
                    .BindTo(SecondsRing, x => x.RenderTransform)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        public static readonly DirectProperty<ClockControl, DateTime> TimeProperty =
            AvaloniaProperty.RegisterDirect<ClockControl, DateTime>(
                nameof(Time),
                o => o.Time,
                (o, v) => o.Time = v);

        private DateTime _time = DateTime.MinValue;
        public DateTime Time
        {
            get => _time;
            set => SetAndRaise(TimeProperty, ref _time, value);
        }
    }
}