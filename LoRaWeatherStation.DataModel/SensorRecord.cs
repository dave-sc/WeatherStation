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

        public override string ToString()
        {
            return $"{{{nameof(Sensor)}: {Sensor}, {nameof(RecordTime)}: {RecordTime}, Values: {Temperature} °C, {Humidity} %RH, {Pressure} hPa, {AirQualityIndex} IAQ, {Luminance} lux, {WindSpeed} km/h}}";
        }
    }
}