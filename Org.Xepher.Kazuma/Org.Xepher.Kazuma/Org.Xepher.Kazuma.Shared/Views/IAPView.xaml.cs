using Org.Xepher.Kazuma.Utils;
using Org.Xepher.Kazuma.ViewModels;
using ReactiveUI;
using Splat;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Org.Xepher.Kazuma.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IAPView : Page, IViewFor<IAPViewModel>, IEnableLogger
    {
        public IAPView()
        {
            this.InitializeComponent();

            InitializeBindingSettings();

            Observable.StartAsync(async () => ProductList.ItemsSource = await IAPHelper.GetProdList());
        }

        #region BasicBinding for View and ViewModel

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IAPViewModel)value; }
        }

        public IAPViewModel ViewModel
        {
            get { return (IAPViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(IAPViewModel), typeof(IAPView), new PropertyMetadata(null));

        private void InitializeBindingSettings()
        {
            // these binding are untestable, don't put complex logic binding in here

            //RxApp.SuspensionHost.ObserveAppState<IAPViewModel>().BindTo(this, x => x.ViewModel);
            
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        #endregion

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await IAPHelper.PurchaseProd();
        }
    }
}
