using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;
using Avalonia.ReactiveUI;
using LoRaWeatherStation.Client;
using LoRaWeatherStation.UserInterface.Configuration;
using LoRaWeatherStation.UserInterface.SystemControl;
using Microsoft.Extensions.Configuration;
using Ninject;
using Splat;
using Splat.Ninject;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace LoRaWeatherStation.UserInterface
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Pi.Init<BootstrapWiringPi>();
            }

            var app = BuildAvaloniaApp(args);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Pi.Init<BootstrapWiringPi>();
                SuppressConsoleCursor();
                Locator.Current.GetService<BacklightControl>().Initialize();
                // On Raspbian additionally depends on libSkiaSharp (shipped via dotnet publish), libfontconfig1 (fontconfig), libinput10
                // Dependencies can be diagnosed by searching for the given file name in the DllNotFoundException or by executing
                // ldd <filename> and looking for missing dependencies of that library
                app.StartLinuxFbDev(args);
            }
            else
            {
                app.StartWithClassicDesktopLifetime(args);
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp(string[] args)
        {
            var kernel = new StandardKernel();
            kernel.UseNinjectDependencyResolver();
            kernel.UseDefaultConfiguration(args);
            ConfigureServices(kernel);
            
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();
        }

        private static void ConfigureServices(IKernel kernel)
        {
            kernel.BindOptions<DashboardOptions>(DashboardOptions.SectionName);
            kernel.BindOptions<BacklightOptions>(BacklightOptions.SectionName);
            kernel.Bind<WeatherStationClient>().ToMethod(ctx => new WeatherStationClient(ctx.Kernel.Get<IConfiguration>().GetValue<string>("WeatherService:ServiceUri"))).InSingletonScope();
            kernel.Bind<IGpioController>().ToMethod(ctx => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Pi.Gpio : null);
        }

        // Required for now to hide console cursor when starting from linux console without desktop environment
        private static void SuppressConsoleCursor()
        {
            // Write magic escape sequences to the main linux terminal, so that the cursor is no longer displayed
            File.WriteAllBytes("/dev/tty1", new byte[]{0x1B, 0x5B, 0x33, 0x4A, 0x1B, 0x5B, 0x3F, 0x32, 0x35, 0x6C});
            Console.BackgroundColor = ConsoleColor.White;
            Console.Clear();
            Console.CursorVisible = false;

            void ThreadStart()
            {
                
                while (true)
                {
                    if (Console.CursorTop > 0)
                        Console.Clear();
                    
                    Thread.Sleep(100);
                }
            }

            var thread = new Thread(ThreadStart)
            {
                Name = "Console Cursor Supression Thread", 
                IsBackground = true,
            };

            thread.Start();
        }
    }
}
