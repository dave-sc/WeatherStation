using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class DashboardView : ReactiveUserControl<DashboardViewModel>
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}