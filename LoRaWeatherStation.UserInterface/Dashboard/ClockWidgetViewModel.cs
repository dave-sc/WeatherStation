using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using LoRaWeatherStation.UserInterface.Infrastructure;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class ClockWidgetViewModel : ViewModelBase
    {
        public ClockWidgetViewModel()
        {
            this.WhenActivated(disposables =>
            {
                _currentTime = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(0.5))
                    .Select(_ => DateTime.Now)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToProperty(this, x => x.CurrentTime)
                    .DisposeWith(disposables);
            });
        }
        
        private ObservableAsPropertyHelper<DateTime> _currentTime;
        
        public DateTime CurrentTime => _currentTime.Value;
    }
}