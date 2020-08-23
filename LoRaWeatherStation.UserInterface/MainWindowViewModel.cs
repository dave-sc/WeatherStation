using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Text;
using LoRaWeatherStation.UserInterface.Dashboard;
using LoRaWeatherStation.UserInterface.Infrastructure;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface
{
    public class MainWindowViewModel : ViewModelBase, IScreen
    {
        public MainWindowViewModel()
            : base(null)
        {
            Pages = new ObservableCollection<IRoutableViewModel>()
            {
                new DashboardViewModel(this)
            };
            
            this.WhenActivated((CompositeDisposable disposables) =>
            {
                Router.Navigate.Execute(new DashboardViewModel(this));
            });
        }
        
        public RoutingState Router { get; } = new RoutingState();
        
        public ObservableCollection<IRoutableViewModel> Pages { get; }
    }
}
