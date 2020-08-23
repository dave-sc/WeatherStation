using Avalonia.Data.Converters;

namespace LoRaWeatherStation.UserInterface.Converters
{
    public class Converters
    {
        public static IValueConverter WeatherTypeToImage { get; } = new WeatherTypeToImageConverter();
    }
}