using System;
using System.Linq;
using System.Reactive.Linq;
using LoRaWeatherStation.UserInterface.Configuration;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace LoRaWeatherStation.UserInterface.SystemControl
{
    public class BacklightControl
    {
        private readonly IGpioController _gpioController;
        private readonly BacklightOptions _options;

        public BacklightControl(IGpioController gpioController, BacklightOptions options)
        {
            _gpioController = gpioController;
            _options = options;
        }

        public void Initialize()
        {
            if (_gpioController == null)
                return;
            
            var backlightPwmPin = (GpioPin)_gpioController[18];
            backlightPwmPin.PinMode = GpioPinDriveMode.PwmOutput;
            backlightPwmPin.PwmMode = PwmMode.Balanced;
            backlightPwmPin.PwmClockDivisor = 2048;
            
            Observable.Timer(TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(1))
                .Select(_ => DateTime.Now)
                .Select(GetDesiredBrightness)
                .Subscribe(brightness => SetBrightness(backlightPwmPin, brightness));
        }

        private double GetDesiredBrightness(DateTime time)
        {
            var todaysSchedule = _options.Schedule?.FirstOrDefault(schedule => schedule.ApplicableDays.Contains(time.DayOfWeek));
            if (todaysSchedule == null)
                return 100;

            var currentScheduleEntry = todaysSchedule.ScheduleEntries?.LastOrDefault(entry => time.TimeOfDay >= entry.StartTime);
            if (currentScheduleEntry == null)
                return 100;

            return currentScheduleEntry.Brightness;
        }

        private void SetBrightness(GpioPin brightnessPin, double brightness)
        {
            brightness = Math.Max(0, Math.Min(100, brightness));
            brightnessPin.PwmRegister = (int)((double)brightnessPin.PwmRange * ((100d - brightness) / 100d));
        }
    }
}