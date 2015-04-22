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
using Splat;
using Windows.UI.Popups;
using ReactiveUI;
using Org.Xepher.Kazuma.Utils;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Org.Xepher.Kazuma
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        private IScreen HostScreen { get; set; }
        private IMessageBus HostMessageBus { get; set; }

        public Shell()
        {
            this.InitializeComponent();

            HostScreen = Locator.Current.GetService<IScreen>();
            HostMessageBus = Locator.Current.GetService<IMessageBus>();

            DataContext = HostScreen;

            HostMessageBus.Listen<string>(Constants.MSGBUS_TOKEN_MESSAGEBAR).Subscribe(x => this.MessageBar.Message = x);

#if WINDOWS_PHONE_APP
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#endif
        }

        #region Navigation

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // RxUI Routing system only use this entrypoint as the first navigation entrypoint
            // if we use IScreen.Router.Navigate or IScreen.Router.NavigateBack ... functions
            // the OnNavigatedTo, OnNavigatedFrom will not be triggered

            // TODO: Prepare page for display here.
            if (!ApplicationDataSettingsHelper.ReadValue<bool>(Constants.SETTINGS_IS_LOCALSTORAGE_ENABLED))
            {
                string message = "WxBus 需要知道您的位置才能正常工作。如果您不允许访问您的位置，请点击\"取消\"，这样将不会启动应用。";

                MessageDialog dialog = new MessageDialog(message, "允许获取位置吗？");
                dialog.Commands.Add(new UICommand("确定", new UICommandInvokedHandler(cmd => { ApplicationDataSettingsHelper.SaveOrUpdateValue<bool>(Constants.SETTINGS_IS_LOCALSTORAGE_ENABLED, true); })));
                dialog.Commands.Add(new UICommand("取消", new UICommandInvokedHandler(cmd => { ApplicationDataSettingsHelper.SaveOrUpdateValue<bool>(Constants.SETTINGS_IS_LOCALSTORAGE_ENABLED, false); App.Current.Exit(); })));
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;

                await dialog.ShowAsync();
            }

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

#if WINDOWS_PHONE_APP
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (this.HostScreen.Router.NavigateBack.CanExecute(null))
            {
                this.HostScreen.Router.NavigateBack.Execute(null);

                e.Handled = true;
            }
        }
#endif

        #endregion
    }
}
