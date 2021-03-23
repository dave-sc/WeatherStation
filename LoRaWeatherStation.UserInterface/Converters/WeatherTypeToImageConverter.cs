using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using WeatherLib;

namespace LoRaWeatherStation.UserInterface.Converters
{
    public class WeatherTypeToImageConverter : IValueConverter
    {
        public static IImage Convert(WeatherType weatherType)
        {
            switch (weatherType)
            {
                case WeatherType.Clear:
                    return Application.Current.FindResource("WeatherIcon.Sun") as IImage;
                case WeatherType.CloudyLight:
                    return Application.Current.FindResource("WeatherIcon.CloudyLight") as IImage;
                case WeatherType.CloudyMedium:
                    return Application.Current.FindResource("WeatherIcon.CloudyMedium") as IImage;
                case WeatherType.CloudyVery:
                    return Application.Current.FindResource("WeatherIcon.CloudyVery") as IImage;
                case WeatherType.Smoke:
                    return Application.Current.FindResource("WeatherIcon.Fog") as IImage;
                case WeatherType.Haze:
                    return Application.Current.FindResource("WeatherIcon.Fog") as IImage;
                case WeatherType.Dust:
                    return Application.Current.FindResource("WeatherIcon.Fog") as IImage;
                case WeatherType.Fog:
                    return Application.Current.FindResource("WeatherIcon.Fog") as IImage;
                case WeatherType.FrostFog:
                    return Application.Current.FindResource("WeatherIcon.FogFreezing") as IImage;
                case WeatherType.LightIntermittentDrizzle:
                    return Application.Current.FindResource("WeatherIcon.DrizzleLight") as IImage;
                case WeatherType.LightDrizzle:
                    return Application.Current.FindResource("WeatherIcon.DrizzleLight") as IImage;
                case WeatherType.IntermittentDrizzle:
                    return Application.Current.FindResource("WeatherIcon.Drizzle") as IImage;
                case WeatherType.Drizzle:
                    return Application.Current.FindResource("WeatherIcon.Drizzle") as IImage;
                case WeatherType.FreezingLightDrizzle:
                    return Application.Current.FindResource("WeatherIcon.DrizzleLightFreezing") as IImage;
                case WeatherType.FreezingDrizzle:
                    return Application.Current.FindResource("WeatherIcon.DrizzleFreezing") as IImage;
                case WeatherType.IntermittentLightRain:
                    return Application.Current.FindResource("WeatherIcon.RainLight") as IImage;
                case WeatherType.LightRain:
                    return Application.Current.FindResource("WeatherIcon.RainLight") as IImage;
                case WeatherType.IntermittentRain:
                    return Application.Current.FindResource("WeatherIcon.Rain") as IImage;
                case WeatherType.Rain:
                    return Application.Current.FindResource("WeatherIcon.Rain") as IImage;
                case WeatherType.IntermittentHeavyRain:
                    return Application.Current.FindResource("WeatherIcon.RainHeavy") as IImage;
                case WeatherType.HeavyRain:
                    return Application.Current.FindResource("WeatherIcon.RainHeavy") as IImage;
                case WeatherType.FreezingLightRain:
                    return Application.Current.FindResource("WeatherIcon.RainLightFreezing") as IImage;
                case WeatherType.FreezingRain:
                    return Application.Current.FindResource("WeatherIcon.RainFreezing") as IImage;
                case WeatherType.LightSleet:
                    return Application.Current.FindResource("WeatherIcon.SleetLight") as IImage;
                case WeatherType.Sleet:
                    return Application.Current.FindResource("WeatherIcon.Sleet") as IImage;
                case WeatherType.IntermittentLightSnow:
                    return Application.Current.FindResource("WeatherIcon.SnowLight") as IImage;
                case WeatherType.LightSnow:
                    return Application.Current.FindResource("WeatherIcon.SnowLight") as IImage;
                case WeatherType.IntermittentSnow:
                    return Application.Current.FindResource("WeatherIcon.Snow") as IImage;
                case WeatherType.Snow:
                    return Application.Current.FindResource("WeatherIcon.Snow") as IImage;
                case WeatherType.Icy:
                    return Application.Current.FindResource("WeatherIcon.Snow") as IImage;
                case WeatherType.LightHail:
                    return Application.Current.FindResource("WeatherIcon.HailLight") as IImage;
                case WeatherType.Hail:
                    return Application.Current.FindResource("WeatherIcon.Hail") as IImage;
                case WeatherType.SnowDrifts:
                    return Application.Current.FindResource("WeatherIcon.Snow") as IImage;
                case WeatherType.Storm:
                    return Application.Current.FindResource("WeatherIcon.Storm") as IImage;
                case WeatherType.Thunder:
                    return Application.Current.FindResource("WeatherIcon.Thunder") as IImage;
                case WeatherType.ThunderStorm:
                    return Application.Current.FindResource("WeatherIcon.ThunderStorm") as IImage;
                case WeatherType.HeavyThunderStorm:
                    return Application.Current.FindResource("WeatherIcon.ThunderStormHeavy") as IImage;
                case WeatherType.ThunderStormHail:
                    return Application.Current.FindResource("WeatherIcon.ThunderStormHail") as IImage;
                case WeatherType.HeavyThunderStormHail:
                    return Application.Current.FindResource("WeatherIcon.ThunderStormHailHeavy") as IImage;
                default:
                    return null;
            }
        }
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WeatherType weatherType)
                return Convert(weatherType) ?? AvaloniaProperty.UnsetValue;

            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}