using System;

namespace LoRaWeatherStation.UserInterface.Configuration
{
    public class BacklightScheduleEntry
    {
        public TimeSpan StartTime { get; set; }
        public double Brightness { get; set; }

        public override string ToString() => $"{StartTime:c}: {Brightness:F2} %";
    }
}