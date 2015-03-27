using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Org.Xepher.Kazuma.Common;
using Org.Xepher.Kazuma.ViewModels;
using ReactiveUI;

namespace Org.Xepher.Kazuma.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page, IViewFor<MainViewModel>
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
            RxApp.SuspensionHost.ObserveAppState<MainViewModel>().BindTo(this, x => x.ViewModel);

            this.Bind(ViewModel, vm => vm.FilterTerm, v => v.FilterTerm.Text);

            this.Bind(ViewModel, vm => vm.Routes, v => v.Routes.ItemsSource);

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        #endregion
    }
}
