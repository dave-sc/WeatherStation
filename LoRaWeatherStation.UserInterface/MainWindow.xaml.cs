using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface
{
    public class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public RoutedViewHost ViewHost => this.FindControl<RoutedViewHost>("ViewHost");
        
        public MainWindow()
        {
            InitializeComponent();
            if (AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem == OperatingSystemType.WinNT)
                SystemDecorations = SystemDecorations.Full;
#if DEBUG
            this.AttachDevTools();
#endif
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