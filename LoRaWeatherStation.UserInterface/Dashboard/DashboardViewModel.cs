using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using LoRaWeatherStation.UserInterface.Infrastructure;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class DashboardViewModel : ViewModelBase
    {
        public DashboardViewModel(IScreen hostScreen)
                : base(hostScreen)
        {
            this.WhenActivated(disposables =>
            {
                _currentTime = Observable.Interval(TimeSpan.FromSeconds(0.5))
                    .Select(x => DateTime.Now)
                    .ObserveOn(SynchronizationContext.Current)
                    .ToProperty(this, x => x.CurrentTime)
                    .DisposeWith(disposables);                
            });
        }
        
        private ObservableAsPropertyHelper<DateTime> _currentTime;
        
        public DateTime CurrentTime => _currentTime.Value;
    }
}