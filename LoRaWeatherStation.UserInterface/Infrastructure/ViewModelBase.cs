using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Infrastructure
{
    public class ViewModelBase : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; }
        
        public ViewModelBase()
        {
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
