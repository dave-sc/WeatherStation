using System;
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
            switch (time.DayOfWeek)
            {
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                case DayOfWeek.Friday:
                    return time.TimeOfDay > _options.WeekDayDimEndTime && time.TimeOfDay < _options.WeekDayDimStartTime ? _options.BacklightBrightnessNormal : _options.BacklightBrightnessDimmed;
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    return time.TimeOfDay > _options.WeekEndDimEndTime && time.TimeOfDay < _options.WeekEndDimStartTime ? _options.BacklightBrightnessNormal : _options.BacklightBrightnessDimmed;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetBrightness(GpioPin brightnessPin, double brightness)
        {
            brightness = Math.Max(0, Math.Min(100, brightness));
            brightnessPin.PwmRegister = (int)((double)brightnessPin.PwmRange * ((100d - brightness) / 100d));
        }
    }
}