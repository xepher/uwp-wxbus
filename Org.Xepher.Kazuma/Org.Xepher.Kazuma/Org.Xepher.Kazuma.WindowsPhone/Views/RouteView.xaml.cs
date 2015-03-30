using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Org.Xepher.Kazuma.ViewModels;
using ReactiveUI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Org.Xepher.Kazuma.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RouteView : Page, IViewFor<RouteViewModel>
    {
        public RouteView()
        {
            this.InitializeComponent();

            InitializeBindingSettings();
        }

        #region BasicBinding for View and ViewModel

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (RouteViewModel)value; }
        }

        public RouteViewModel ViewModel
        {
            get { return (RouteViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(RouteViewModel), typeof(RouteView), new PropertyMetadata(null));

        private void InitializeBindingSettings()
        {
            this.Bind(ViewModel, vm => vm.Segments, v => v.Segments.ItemsSource);

            this.Bind(ViewModel, vm => vm.SelectedSegmentIndex, v => v.Segments.SelectedIndex);

            this.BindCommand(ViewModel, vm => vm.RefreshCommand, v => v.Refresh);

            this.BindCommand(ViewModel, vm => vm.SearchCommand, v => v.Search);

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        #endregion
    }
}
