using System;
using System.Collections.Generic;

namespace LoRaWeatherStation.UserInterface.Configuration
{
    public class BacklightSchedule
    {
        public ICollection<DayOfWeek> ApplicableDays { get; } = new List<DayOfWeek>();
        public ICollection<BacklightScheduleEntry> ScheduleEntries { get; } = new List<BacklightScheduleEntry>();
    }
}