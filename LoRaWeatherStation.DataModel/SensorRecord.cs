using System;
using System.ComponentModel.DataAnnotations;

namespace LoRaWeatherStation.DataModel
{
    public class SensorRecord
    {
        [Key]
        public long Id { get; set; }
        
        public Sensor Sensor { get; set; }
        
        public DateTime RecordTime { get; set; }
        
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float Pressure { get; set; }
        public float AirQualityIndex { get; set; }
        public float Luminance { get; set; }
        public float WindSpeed { get; set; }
        
        public float GetValue(SensorValues value)
        {
            switch (value)
            {
                case SensorValues.None:
                    return float.NaN;
                case SensorValues.Temperature:
                    return Temperature;
                case SensorValues.Humidity:
                    return Humidity;
                case SensorValues.Pressure:
                    return Pressure;
                case SensorValues.AirQualityIndex:
                    return AirQualityIndex;
                case SensorValues.Luminance:
                    return Luminance;
                case SensorValues.WindSpeed:
                    return WindSpeed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        public override string ToString()
        {
            return $"{{{nameof(Sensor)}: {Sensor}, {nameof(RecordTime)}: {RecordTime}, Values: {Temperature} °C, {Humidity} %RH, {Pressure} hPa, {AirQualityIndex} IAQ, {Luminance} lux, {WindSpeed} km/h}}";
        }
    }
}