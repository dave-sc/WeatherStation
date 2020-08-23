using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Infrastructure
{
    public class ViewModelBase : ReactiveObject, IActivatableViewModel, IRoutableViewModel
    {
        public ViewModelActivator Activator { get; }
        
        public IScreen HostScreen { get; }
        
        public string UrlPathSegment { get; }
        
        public ViewModelBase(IScreen hostScreen)
        {
            HostScreen = hostScreen;
            UrlPathSegment = GetType().Name;
            
            Activator = new ViewModelActivator();
            this.WhenActivated(disposables =>
            {
                HandleActivation();
                Disposable
                    .Create(HandleDeactivation)
                    .DisposeWith(disposables);
            });
        }
        
        protected virtual void HandleActivation() { }
    
        protected virtual  void HandleDeactivation() { }
    }
}
