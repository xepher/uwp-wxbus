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
using Caliburn.Micro;
using Org.Xepher.Kazuma.Common;
using Org.Xepher.Kazuma.ViewModels;
using ReactiveUI;

namespace Org.Xepher.Kazuma.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        public MainView()
        {
            this.InitializeComponent();

            var keyUp = Observable.FromEventPattern<KeyRoutedEventArgs>(SearchBox, "KeyUp")
                .Select(evt => ((TextBox) evt.Sender).Text)
                .Throttle(TimeSpan.FromSeconds(0.5));

            MessageBus.Current.RegisterMessageSource(keyUp);
        }
    }
}
