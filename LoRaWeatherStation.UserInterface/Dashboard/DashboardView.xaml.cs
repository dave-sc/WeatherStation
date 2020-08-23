using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LoRaWeatherStation.UserInterface.Controls;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class DashboardView : ReactiveUserControl<DashboardViewModel>
    {
        private ClockControl ClockControl => this.FindControl<ClockControl>("Clock");
        
        public DashboardView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.CurrentTime, v => v.ClockControl.Time)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}