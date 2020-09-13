using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Infrastructure
{
    public class PageViewModel : ViewModelBase, IRoutableViewModel
    {
        public IScreen HostScreen { get; }
        
        public string UrlPathSegment { get; }

        public PageViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
            UrlPathSegment = GetType().Name;
        }
    }
}