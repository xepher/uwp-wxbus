using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Controls.Maps.Core;
using Microsoft.Phone.Shell;
using org.xepher.control;
using org.xepher.lang;
using org.xepher.model;
using org.xepher.wuxibus.misc;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;
using GoogleMapsTileSource = org.xepher.wuxibus.misc.MapsTileSource.GoogleMapsTileSource.MapsRoadTileSource;
using NokiaMapsTileSource = org.xepher.wuxibus.misc.MapsTileSource.NokiaMapsTileSource.MapsRoadTileSource;

namespace org.xepher.wuxibus
{
    public partial class LocatePage : PhoneApplicationPage
    {
        private App _app;

        // Register in Microsoft
        // use this in webservice
        //static string token = "4ac548ed-ffb6-4636-ad67-324e17147841";
        //static string appId = "GJ7htvqp_lX8NsBIMaTrJw";

        // map control ApllicationId
        private const string _applicationId = "ApStfI1_pvzq1cGC04bdjzzLE_K02JnoYHxPJbeOyrQBGxZvCAMze-naiUoQExf4";
        private GeoCoordinateWatcher _geoCoordinateWatcher;
        private Pushpin _myLocation;
        private Popup _popupLoading;
        private Popup _popupLines;
        private UIElement _border;
        private int _myLocationIndex;
        private bool _isGPSAvailable;

        private readonly ImageBrush inactivedBtnBrush = new ImageBrush()
            {
                ImageSource =
                    new BitmapImage(new Uri("/Assets/Button/mypos_inactive.png",
                                            UriKind.Relative)),
                Stretch = Stretch.Fill
            };

        private MapTileLayer nokiatileLayer;
        private MapTileLayer googletileLayer;

        public LocatePage()
        {
            InitializeComponent();

            InitializeMaps();

            ApplicationBarLocalization();

            InitializePopup();
        }

        private void InitializeMaps()
        {
            _app = (Application.Current as App);
            btnMyLocation.Click += btnMyLocation_Click;

            mapLocate.CredentialsProvider = new ApplicationIdCredentialsProvider(_applicationId);
            mapLocate.LogoVisibility = Visibility.Collapsed;
            mapLocate.CopyrightVisibility = Visibility.Collapsed;
            mapLocate.Mode = new MercatorMode();// 使用bingmap时设置为MercatorMode,google和nokia可以设置为RoadMode及AerialMode等MercatorMode子类
            mapLocate.ZoomBarVisibility = Visibility.Collapsed;
            mapLocate.ZoomLevel = 11;
            mapLocate.Center = GeoCoordinateHelper.AddOffset(new GeoCoordinate(31.57, 120.30));
            mapLocate.MapZoom += mapLocate_MapZoom;

            // 添加默认图层
            nokiatileLayer = new MapTileLayer();
            TileSource nokiatileSource = new NokiaMapsTileSource();
            nokiatileLayer.TileSources.Add(nokiatileSource);
            nokiatileLayer.Opacity = 1;

            googletileLayer = new MapTileLayer();
            TileSource googletileSource = new GoogleMapsTileSource();
            googletileLayer.TileSources.Add(googletileSource);
            googletileLayer.Opacity = 1;

            mapLocate.Children.Add(nokiatileLayer);

            _geoCoordinateWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default) { MovementThreshold = 2 };
            _geoCoordinateWatcher.PositionChanged += _geoCoordinateWatcher_PositionChanged;
            _geoCoordinateWatcher.StatusChanged += _geoCoordinateWatcher_StatusChanged;

            _isGPSAvailable = _geoCoordinateWatcher.TryStart(true, TimeSpan.FromMilliseconds(1000));
        }

        private void ApplicationBarLocalization()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

            // add buttons
            ApplicationBarIconButton addSwitchRoadTileButton = new ApplicationBarIconButton()
                                                                   {
                                                                       Text = AppResource.ApplicationBarIconButtonSwichRoadTile,
                                                                       IconUri =
                                                                           new Uri(
                                                                           "/Assets/Icons/dark/appbar.refresh.rest.png",
                                                                           UriKind.Relative)
                                                                   };
            addSwitchRoadTileButton.Click += new EventHandler(addSwitchRoadTileButton_Click);

            ApplicationBar.Buttons.Add(addSwitchRoadTileButton);

            ApplicationBarIconButton minusTileButton = new ApplicationBarIconButton()
                                                           {
                                                               Text = AppResource.ApplicationBarIconButtonMinusTile,
                                                               IconUri =
                                                                   new Uri(
                                                                   "/Assets/Icons/dark/appbar.minus.rest.png",
                                                                   UriKind.Relative)
                                                           };
            minusTileButton.Click += new EventHandler(minusTileButton_Click);

            ApplicationBar.Buttons.Add(minusTileButton);

            ApplicationBarIconButton addStationsTileButton = new ApplicationBarIconButton()
                                                                 {
                                                                     Text = AppResource.ApplicationBarIconButtonAddStationsTile,
                                                                     IconUri =
                                                                         new Uri(
                                                                         "/Assets/Icons/dark/appbar.add.rest.png",
                                                                         UriKind.Relative),
                                                                     IsEnabled = false
                                                                 };
            addStationsTileButton.Click += new EventHandler(addStationsTileButton_Click);

            ApplicationBar.Buttons.Add(addStationsTileButton);

            ApplicationBar.Opacity = 0.9;
        }

        private void InitializePopup()
        {
            _popupLoading = new Popup();
            _popupLines = new Popup();

            _border = new UC_PopupLoading();

            _popupLoading.Child = _border;
        }

        private void minusTileButton_Click(object sender, EventArgs e)
        {
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;

            var tileLayer = mapLocate.Children[0];
            mapLocate.Children.Clear();
            mapLocate.Children.Add(tileLayer);

#if DEBUG
            ToastPrompt toast = new ToastPrompt();
            toast.Message = "图层数:" + mapLocate.Children.Count;
            toast.Show();
#endif
        }

        private void addSwitchRoadTileButton_Click(object sender, EventArgs e)
        {
            if ((mapLocate.Children[0] as MapTileLayer).TileSources[0] is NokiaMapsTileSource)
                mapLocate.Children[0] = googletileLayer;
            else
                mapLocate.Children[0] = nokiatileLayer;

#if DEBUG
            ToastPrompt toast = new ToastPrompt();
            toast.Message = "图层数:" + mapLocate.Children.Count;
            toast.Show();
#endif
        }

        private void addStationsTileButton_Click(object sender, EventArgs e)
        {
            _popupLoading.IsOpen = true;
            ApplicationBar.IsVisible = false;
            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = false;

            new Thread(() =>
                {
                    // 获取指定范围内所有站台
                    List<Station> allStations =
                        _app.DAHelperInstance.GetStationsByLocation(
                            _geoCoordinateWatcher.Position.Location.Longitude,
                            _geoCoordinateWatcher.Position.Location.Latitude, 0.004,
                            0.004);

                    if (allStations == null) return;

                    var stations =
                        allStations.GroupBy(s => s.station_smsid).Select(
                            stationGroup => stationGroup.ToList()).ToList();

                    Dispatcher.BeginInvoke(() =>
                        {
                            mapLocate.ZoomLevel = 16;

                            foreach (var stationGroup in stations)
                            {
                                if (stationGroup.Count() > 1)
                                {
                                    Pushpin _stationPushPin = new Pushpin();
                                    _stationPushPin.FontSize = 30;
                                    _stationPushPin.Location =
                                        GeoCoordinateHelper.AddOffset(
                                            new GeoCoordinate(double.Parse(stationGroup[0].wd_str),
                                                              double.Parse(stationGroup[0].jd_str)));

                                    _stationPushPin.Content = stationGroup[0].station_name;
                                    _stationPushPin.Tap += _stationPushPin_Tap;
                                    mapLocate.Children.Add(_stationPushPin);
                                }
                            }

#if DEBUG
                            ToastPrompt toast = new ToastPrompt();
                            toast.Message = "图层数:" + mapLocate.Children.Count;
                            toast.Show();
#endif

                            (ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
                            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
                            ApplicationBar.IsVisible = true;
                            _popupLoading.IsOpen = false;
                        });
                }).Start();
        }

        private void _stationPushPin_Tap(object sender, GestureEventArgs e)
        {
            GeoCoordinate location = GeoCoordinateHelper.MinusOffset((sender as Pushpin).Location);

            _popupLoading.IsOpen = true;
            ApplicationBar.IsVisible = false;

            new Thread(() =>
                {
                    List<Station> stations = _app.DAHelperInstance.GetStationByLocation(location.Longitude,
                                                                                        location.Latitude);

                    Dispatcher.BeginInvoke(() =>
                        {
                            PopupLines(stations);

                            _popupLoading.IsOpen = false;
                        });
                }).Start();
        }

        private void PopupLines(List<Station> stations)
        {
            StackPanel _stackLines = new StackPanel();
            _stackLines.Width = LayoutRoot.ActualWidth;
            _stackLines.Height = 800;
            _stackLines.HorizontalAlignment = HorizontalAlignment.Stretch;
            _stackLines.VerticalAlignment = VerticalAlignment.Stretch;
            _stackLines.Background = new SolidColorBrush(Colors.Gray);

            _popupLines.Child = _stackLines;

            Brush borderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x9B, 0xE3));
            foreach (var station in stations)
            {
                UC_LineInfo line = new UC_LineInfo(station);
                line.Border.Background = borderBrush;
                line.Tap += line_Tap;
                _stackLines.Children.Add(line);
            }

            _popupLines.IsOpen = true;
        }

        private void line_Tap(object sender, GestureEventArgs e)
        {
            _popupLines.IsOpen = false;
            ApplicationBar.IsVisible = true;
            _app.SelectedLine = (sender as UC_LineInfo).Line;
            NavigationService.Navigate(new Uri("/StationPage.xaml", UriKind.Relative));
        }

        private void btnMyLocation_Click(object sender, RoutedEventArgs e)
        {
            mapLocate.ZoomLevel = 16;
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;

            if (btnMyLocation.Background == inactivedBtnBrush) return;
            Brush activedBtnBrush = btnMyLocation.Background;

            btnMyLocation.Background = inactivedBtnBrush;

            var t = new Thread(() =>
                {
                    _geoCoordinateWatcher.TryStart(true, TimeSpan.FromSeconds(30));
                    Dispatcher.BeginInvoke(() =>
                        {
                            if (_geoCoordinateWatcher.Status == GeoPositionStatus.Ready)
                            {
                                if (_myLocationIndex == 0)
                                {
                                    _myLocation = new Pushpin()
                                        {
                                            Location = GeoCoordinateHelper.AddOffset(_geoCoordinateWatcher.Position.Location),
                                            Content = "Me",
                                            FontSize = 30
                                        };
                                    mapLocate.Children.Add(_myLocation);
                                    _myLocationIndex = mapLocate.Children.Count - 1;

                                    mapLocate.Center = _myLocation.Location;
                                }
                            }

                            btnMyLocation.Background = activedBtnBrush;

#if DEBUG
                            ToastPrompt toast = new ToastPrompt();
                            toast.Message = "图层数:" + mapLocate.Children.Count;
                            toast.Show();
#endif
                        });
                });
            t.Start();
        }

        private void mapLocate_MapZoom(object sender, MapZoomEventArgs e)
        {
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = mapLocate.ZoomLevel > 15;
#if DEBUG
            ToastPrompt toast = new ToastPrompt();
            toast.Message = "ZoomLevel:" + mapLocate.ZoomLevel.ToString();
            toast.Show();
#endif
        }

        // 位置改变
        private void _geoCoordinateWatcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (_myLocationIndex != 0)
            {
                _myLocation = new Pushpin()
                {
                    Location = GeoCoordinateHelper.AddOffset(_geoCoordinateWatcher.Position.Location),
                    Content = "Me",
                    FontSize = 30
                };
                mapLocate.Children[_myLocationIndex] = _myLocation;

                mapLocate.Center = _myLocation.Location;
            }
#if DEBUG
            ToastPrompt toast = new ToastPrompt();
            toast.Message = "纬度:" + e.Position.Location.Latitude.ToString() + "    经度:" + e.Position.Location.Longitude.ToString();
            toast.Show();
#endif
        }

        // GPS状态改变
        private void _geoCoordinateWatcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            if (e.Status == GeoPositionStatus.Ready)
            {

            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_popupLines.IsOpen)
            {
                _popupLines.IsOpen = false;
                ApplicationBar.IsVisible = true;
                e.Cancel = true;
            }
        }

        private void LocatePage_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_isGPSAvailable)
            {
                btnMyLocation.IsEnabled = false;

                ToastPrompt toast = new ToastPrompt();
                toast.Message = AppResource.MsgGPSServiceNotOpen;
                toast.Show();
            }
        }
    }
}