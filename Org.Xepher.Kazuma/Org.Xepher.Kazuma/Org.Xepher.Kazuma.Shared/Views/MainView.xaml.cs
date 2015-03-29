using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Org.Xepher.Kazuma.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Reactive.Linq;

namespace Org.Xepher.Kazuma.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page, IViewFor<MainViewModel>, IEnableLogger
    {
        public MainView()
        {
            this.InitializeComponent();

            InitializeBindingSettings();
        }

        #region BasicBinding for View and ViewModel

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel)value; }
        }

        public MainViewModel ViewModel
        {
            get { return (MainViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainView), new PropertyMetadata(null));

        private void InitializeBindingSettings()
        {
            // these binding are untestable, don't put complex logic binding in here

            //RxApp.SuspensionHost.ObserveAppState<MainViewModel>().BindTo(this, x => x.ViewModel);

            this.Bind(ViewModel, vm => vm.FilterTerm, v => v.FilterTerm.Text);

            this.Bind(ViewModel, vm => vm.Routes, v => v.Routes.ItemsSource);

            this.Bind(ViewModel, vm => vm.SelectedRoute, v => v.Routes.SelectedItem);

            //this.BindCommand(ViewModel, vm => vm.ClearCacheAsyncCommand, v => v.ClearCache);

            //this.WhenAnyObservable(v => v.ViewModel.ClearCacheAsyncCommand).Subscribe(_ =>
            //{
            //    LogHost.Default.Debug("button clicked!!!");
            //});

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        #endregion
    }
}
