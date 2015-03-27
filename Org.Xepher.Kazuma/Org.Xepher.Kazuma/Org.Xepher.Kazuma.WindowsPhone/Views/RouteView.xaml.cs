using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Org.Xepher.Kazuma.ViewModels;
using ReactiveUI;
using System.Reactive.Linq;
using System;
using Windows.Phone.UI.Input;

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
            RxApp.SuspensionHost.ObserveAppState<RouteViewModel>().BindTo(this, x => x.ViewModel);

            this.Bind(ViewModel, vm => vm.Segments, v => v.Segments.ItemsSource);

            this.Bind(ViewModel, vm => vm.SelectedSegmentIndex, v => v.Segments.SelectedIndex);
            
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (ViewModel.HostScreen.Router.NavigateBack.CanExecute(null))
            {
                e.Handled = true;

                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;

                ViewModel.HostScreen.Router.NavigateBack.Execute(null);
            }

        }

        #endregion
    }
}
