using Org.Xepher.Kazuma.Common;
using Org.Xepher.Kazuma.Utils;
using ReactiveUI;
using Splat;
using System;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Org.Xepher.Kazuma
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        private IAppBootstrapper hostAppBootstrapper { get; set; }
        private IMessageBus hostMessageBus { get; set; }

        public Shell()
        {
            this.InitializeComponent();

            hostAppBootstrapper = Locator.Current.GetService<IAppBootstrapper>();
            hostMessageBus = Locator.Current.GetService<IMessageBus>();

            DataContext = hostAppBootstrapper;

            hostMessageBus.Listen<string>(Constants.MSGBUS_TOKEN_MESSAGEBAR).Subscribe(x => this.MessageBar.Message = x);

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
            if (!ApplicationDataSettingsHelper.ReadValue<bool>(Constants.SETTINGS_IS_LOCATION_ENABLED))
            {
                string message = "WxBus 需要知道您的位置信息才能正常工作。\r\n\r\n您提供的位置信息仅会提交给数据供应商，用于追踪及查询，本应用不会搜集使用您的位置信息。\r\n\r\n如果您不允许我们访问您的位置信息，请点击\"取消\"，这样将不会启动应用。";

                MessageDialog dialog = new MessageDialog(message, "允许获取位置信息吗？");
                dialog.Commands.Add(new UICommand("确定", new UICommandInvokedHandler(async cmd =>
                {
#if DEBUG
                    ApplicationDataSettingsHelper.SaveOrUpdateValue<bool>(Constants.SETTINGS_IS_LOCATION_ENABLED, false);
#else
                    ApplicationDataSettingsHelper.SaveOrUpdateValue<bool>(Constants.SETTINGS_IS_LOCATION_ENABLED, true);
#endif

                    hostMessageBus.SendMessage<string>(Constants.MSG_MAP_LOCATION_GET, Constants.MSGBUS_TOKEN_MESSAGEBAR);

                    Geolocator geolocator = new Geolocator { ReportInterval = 1000, DesiredAccuracy = PositionAccuracy.High, DesiredAccuracyInMeters = 10, MovementThreshold = 5 };
                    geolocator.StatusChanged += geolocator_StatusChanged;
                    if (geolocator.LocationStatus == PositionStatus.Ready)
                    {
                        Geoposition location = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(5));
                        hostMessageBus.SendMessage<BasicGeoposition>(location.Coordinate.Point.Position, Constants.MSGBUS_TOKEN_MY_GEOPOSITION);
                    }
                })));
                dialog.Commands.Add(new UICommand("取消", new UICommandInvokedHandler(cmd => { ApplicationDataSettingsHelper.SaveOrUpdateValue<bool>(Constants.SETTINGS_IS_LOCALSTORAGE_ENABLED, false); App.Current.Exit(); })));
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;

                await dialog.ShowAsync();
            }
            else
            {
                hostMessageBus.SendMessage<string>(Constants.MSG_MAP_LOCATION_GET, Constants.MSGBUS_TOKEN_MESSAGEBAR);

                Geolocator geolocator = new Geolocator { ReportInterval = 1000, DesiredAccuracy = PositionAccuracy.High, DesiredAccuracyInMeters = 10, MovementThreshold = 5 };
                geolocator.StatusChanged += geolocator_StatusChanged;
                if (geolocator.LocationStatus == PositionStatus.Ready)
                {
                    Geoposition location = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(5));
                    hostMessageBus.SendMessage<BasicGeoposition>(location.Coordinate.Point.Position, Constants.MSGBUS_TOKEN_MY_GEOPOSITION);
                }
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
            if (this.hostAppBootstrapper.Router.NavigateBack.CanExecute(null))
            {
                this.hostAppBootstrapper.Router.NavigateBack.Execute(null);

                e.Handled = true;
            }
        }
#endif

        #endregion

        void geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            switch (args.Status)
            {
                case PositionStatus.Ready:
                    // Location platform is providing valid data.
                    break;

                case PositionStatus.Initializing:
                    // Location platform is acquiring a fix. It may or may not have data. Or the data may be less accurate.
                    break;

                case PositionStatus.NoData:
                    // Location platform could not obtain location data.
                    break;

                case PositionStatus.Disabled:
                    // The permission to access location data is denied by the user or other policies.
                    hostMessageBus.SendMessage<string>(Constants.MSG_MAP_LOCATION_SERVICE_UNAVAILABLE, Constants.MSGBUS_TOKEN_MESSAGEBAR);
                    break;

                case PositionStatus.NotInitialized:
                    // The location platform is not initialized. This indicates that the application has not made a request for location data.
                    break;

                case PositionStatus.NotAvailable:
                    // The location platform is not available on this version of the OS.
                    break;

                default:
                    // Unknown
                    break;
            }
        }
    }
}
