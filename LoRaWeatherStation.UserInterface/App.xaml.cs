using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace LoRaWeatherStation.UserInterface
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            switch (ApplicationLifetime)
            {
                case null: // Avalonia Designer extension for Rider initializes app without lifetime object
                    break;
                case IClassicDesktopStyleApplicationLifetime desktop:
                    desktop.MainWindow = new MainWindow {DataContext = new MainViewModel()};
                    break;
                case ISingleViewApplicationLifetime sva:
                    sva.MainView = new MainView { DataContext = new MainViewModel()};
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ApplicationLifetime), $"Can not start application with lifetime of type {ApplicationLifetime.GetType().Name}");
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}