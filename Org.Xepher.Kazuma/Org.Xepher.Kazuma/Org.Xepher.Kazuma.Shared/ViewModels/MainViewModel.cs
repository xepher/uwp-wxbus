using Caliburn.Micro;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Routes = new BindableCollection<RouteCardViewModel>
            {
                new RouteCardViewModel("1"),
                new RouteCardViewModel("2"),
                new RouteCardViewModel("3"),
                new RouteCardViewModel("4"),
                new RouteCardViewModel("5"),
                new RouteCardViewModel("6"),
                new RouteCardViewModel("7"),
                new RouteCardViewModel("8"),
                new RouteCardViewModel("9")
            };
        }
        
        public BindableCollection<RouteCardViewModel> Routes
        {
            get;
            private set;
        }
    }
}
