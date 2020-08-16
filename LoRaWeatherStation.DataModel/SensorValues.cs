using System;

namespace LoRaWeatherStation.DataModel
{
    [Flags]
    public enum SensorValues
    {
        None = 0,
        Temperature = 1,
        Humidity = 2,
        Pressure = 4,
        AirQualityIndex = 8,
        Luminance = 16,
        WindSpeed = 32
    }
}