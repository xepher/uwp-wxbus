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
using ReactiveUI;
using Splat;
using Org.Xepher.Kazuma.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Org.Xepher.Kazuma.Views.UserControls
{
    public sealed partial class License : UserControl
    {
        public License()
        {
            this.InitializeComponent();
        }

        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            IScreen hostScreen = Locator.Current.GetService<IScreen>();
            IMessageBus hostMessageBus = Locator.Current.GetService<IMessageBus>();
            hostScreen.Router.Navigate.Execute(new IAPViewModel(hostScreen, hostMessageBus));
        }
    }
}
