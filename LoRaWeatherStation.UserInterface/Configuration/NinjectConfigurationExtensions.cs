using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Ninject;
using Ninject.Syntax;

namespace LoRaWeatherStation.UserInterface.Configuration
{
    public static class NinjectConfigurationExtensions
    {
        public static void UseDefaultConfiguration(this IBindingRoot kernel, string[] args = null)
        {
            kernel.UseConfiguration(config =>
            {
                var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

#if DEBUG
                config.AddUserSecrets(Assembly.GetEntryAssembly(), true);
#endif

                config.AddEnvironmentVariables();

                if (args != null)
                    config.AddCommandLine(args);
            });
        }

        public static void UseConfiguration(this IBindingRoot kernel, Action<ConfigurationBuilder> configureDelegate)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
            configureDelegate(configBuilder);

            var config = configBuilder.Build();
            kernel.Bind<IConfigurationRoot, IConfiguration>().ToConstant(config);
        }

        public static IBindingWhenInNamedWithOrOnSyntax<TOptions> BindOptions<TOptions>(this IBindingRoot kernel, string sectionName)
        {
            return kernel.Bind<TOptions>().ToMethod(ctx => ctx.Kernel.Get<IConfiguration>().GetSection(sectionName).Get<TOptions>());
        }
    }
}