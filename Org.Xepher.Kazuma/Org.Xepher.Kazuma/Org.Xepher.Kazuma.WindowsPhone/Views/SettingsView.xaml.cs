using Org.Xepher.Kazuma.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Org.Xepher.Kazuma.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page ,IViewFor<SettingsViewModel>
    {
        public SettingsView()
        {
            this.InitializeComponent();

            InitializeBindingSettings();
        }

        #region BasicBinding for View and ViewModel

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (SettingsViewModel)value; }
        }

        public SettingsViewModel ViewModel
        {
            get { return (SettingsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SettingsViewModel), typeof(SettingsView), new PropertyMetadata(null));

        private void InitializeBindingSettings()
        {
            this.Bind(ViewModel, vm => vm.IsLocalStorageOn, v => v.LocalStorage.IsOn);

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        #endregion

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
