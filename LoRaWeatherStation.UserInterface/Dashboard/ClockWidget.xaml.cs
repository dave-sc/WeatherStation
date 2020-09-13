using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LoRaWeatherStation.UserInterface.Controls;
using ReactiveUI;
using Splat;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class ClockWidget : ReactiveUserControl<ClockWidgetViewModel>
    {
        private ClockControl ClockControl => this.FindControl<ClockControl>("Clock");
        
        public ClockWidget()
        {
            DataContext = Locator.Current.GetService<ClockWidgetViewModel>();
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.CurrentTime, v => v.ClockControl.Time)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}