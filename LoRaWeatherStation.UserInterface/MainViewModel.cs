using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Text;
using LoRaWeatherStation.UserInterface.Configuration;
using LoRaWeatherStation.UserInterface.Dashboard;
using LoRaWeatherStation.UserInterface.Infrastructure;
using ReactiveUI;
using Splat;

namespace LoRaWeatherStation.UserInterface
{
    public class MainViewModel : ViewModelBase, IScreen
    {
        public MainViewModel()
        {
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                Router.Navigate.Execute(new DashboardViewModel(this, Locator.Current.GetService<DashboardOptions>()));
            });
        }
        
        public RoutingState Router { get; } = new RoutingState();
    }
}
