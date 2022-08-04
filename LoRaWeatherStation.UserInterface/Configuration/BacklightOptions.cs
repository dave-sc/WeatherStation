using System;
using System.Collections.Generic;

namespace LoRaWeatherStation.UserInterface.Configuration
{
    public class BacklightOptions
    {
        public const string SectionName = "Backlight";

        public ICollection<BacklightSchedule> Schedule { get; } = new List<BacklightSchedule>();
    }
}