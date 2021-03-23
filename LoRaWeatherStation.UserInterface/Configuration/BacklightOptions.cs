using System;

namespace LoRaWeatherStation.UserInterface.Configuration
{
    public class BacklightOptions
    {
        public const string SectionName = "Backlight";
        
        public double BacklightBrightnessNormal { get; set; }
        public double BacklightBrightnessDimmed { get; set; }
        public TimeSpan WeekDayDimStartTime { get; set; }
        public TimeSpan WeekDayDimEndTime { get; set; }
        public TimeSpan WeekEndDimStartTime { get; set; }
        public TimeSpan WeekEndDimEndTime { get; set; }
    }
}