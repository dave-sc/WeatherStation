using System;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface
{
    public class MainView : ReactiveUserControl<MainViewModel>
    {
        public RoutedViewHost ViewHost => this.FindControl<RoutedViewHost>("ViewHost");
        
        public MainView()
        {
            InitializeComponent();
            
            this.WhenActivated(disposables =>
            {
                this.Bind(ViewModel, vm => vm.Router, v => v.ViewHost.Router)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}