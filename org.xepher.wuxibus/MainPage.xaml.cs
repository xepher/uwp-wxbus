using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using org.xepher.common;
using org.xepher.control;
using org.xepher.lang;
using org.xepher.model;
using org.xepher.wuxibus.misc;

namespace org.xepher.wuxibus
{
    public partial class MainPage : PhoneApplicationPage
    {
        private App _app;
        private BackgroundWorker _backroungWorker;
        private Popup _splashScreenPopup;
        private List<Line> _lstLine;
        private About _about;
        private int _restoreTime;

        public MainPage()
        {
            InitializeComponent();

            _app = (Application.Current as App);
            _about = new About();

            InitializeResources();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

#if !DEBUG
            _app.AdHelperInstance.InitializeAds(LayoutRoot, panoramaContainer);
#endif

            base.OnNavigatedTo(e);
        }

        private void InitializeResources()
        {
            _splashScreenPopup = new Popup()
                {
                    IsOpen = true,
                    Child = new UC_SplashScreen()
                        {
                            Text = AppResource.PopupLoadingText
                        }
                };

            _backroungWorker = new BackgroundWorker();
            RunBackgroundWorker();
        }

        #region 加载线路
        private void RunBackgroundWorker()
        {
            _backroungWorker.DoWork += ((s, args) =>
            {
                Dispatcher.BeginInvoke(
                    () => { (_splashScreenPopup.Child as UC_SplashScreen).Text = AppResource.LoadingTextReleaseDB; });

#if DEBUG
                // 测试自动更新用
                AppSettingHelper.AddOrUpdateValue(StringConstants.VERSION_CODE, Int32Constants.VERSION_CODE_TEST);
#endif

                // 释放zip到IsolatedStorage
                IsolatedStorage.Zip2IS(ReleaseResource.GetResource("/org.xepher.wuxibus;component/Data/wuxitraffic.zip"),
                                       AppSettingHelper.GetValueOrDefault(StringConstants.VERSION_CODE, Int32Constants.VERSION_CODE) < Int32Constants.VERSION_CODE);

                FetchLines();
            });

            _backroungWorker.RunWorkerCompleted += ((s, args) => Dispatcher.BeginInvoke(() =>
                {
                    ImageSource source = new BitmapImage(new Uri("/Assets/Images/bus.png", UriKind.Relative));
                    ImageBrush brush = new ImageBrush()
                        {
                            ImageSource =
                                new BitmapImage(new Uri("/Assets/Icons/dark/appbar.statusPost.Pin.png", UriKind.Relative)),
                            Stretch = Stretch.None
                        };
                    Brush borderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x9B, 0xE3));
                    Brush backBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x23, 0x34));
                    foreach (Line line in _lstLine)
                    {
                        UC_Line _line = new UC_Line();
                        _line.ImageSource = source;
                        _line.Border.Background = borderBrush;
                        _line.Text = line.line_name;
                        _line.Grid.Tag = line;
                        _line.Grid.Tap += _line_Tap;
                        _line.Image.Tag = line;
                        _line.Image.Tap += _imgPin_OnTap;
                        _line.Image.Background = backBrush;
                        (_line.Image.Children[0] as ImageTile).Background = brush;

                        linesList.Items.Add(_line);
                    }

                    _splashScreenPopup.IsOpen = false;
                }));

            _backroungWorker.RunWorkerAsync();
        }

        private void FetchLines()
        {
            try
            {
                Dispatcher.BeginInvoke(
                    () => { (_splashScreenPopup.Child as UC_SplashScreen).Text = AppResource.LoadingTextPrepareLines; });

                _lstLine = _app.DAHelperInstance.GetAllLine();
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(
                    () => { (_splashScreenPopup.Child as UC_SplashScreen).Text = string.Format(AppResource.LoadingTextDBDestroyed, ++_restoreTime); });

                _app.DAHelperInstance.DisposeConnection();

                // 如果数据库出错的话会在这里捕捉到异常
                // 这里需要将自带数据库释放
                IsolatedStorage.Zip2IS(
                    ReleaseResource.GetResource("/org.xepher.wuxibus;component/Data/wuxitraffic.zip"), true);

                _app.DAHelperInstance.OpenConnection();

                if (_restoreTime < 3)
                    FetchLines();
                else
                    throw ex;
            }
        }

        private void _imgPin_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Line line = (Line)(sender as Grid).Tag;
            string url = string.Format("/StationPage.xaml?id={0}", line.line_id);

            PinHelper.PinToStart(url, line.line_name);
        }

        private void _line_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _app.SelectedLine = ((sender as Grid).Tag as Line);
            NavigationService.Navigate(new Uri("/StationPage.xaml", UriKind.Relative));
        }
        #endregion

        #region 其他
        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void BtnLocate_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LocatePage.xaml", UriKind.Relative));
        }

        private void BtnTrafficInfo_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/TrafficInfoPage.xaml", UriKind.Relative));
        }

        private void BtnSetting_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }

        private void BtnAbout_OnClick(object sender, RoutedEventArgs e)
        {
            _about = new About();
            _about.Show();
        }
        #endregion

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_about.AboutPrompt.IsOpen)
            {
                MessageBoxResult result = MessageBox.Show(AppResource.MsgExitApplication,
                                                          AppResource.TitleExitApplication,
                                                          MessageBoxButton.OKCancel);
                if (MessageBoxResult.Cancel == result)
                {
                    e.Cancel = true;
                }
                else if (MessageBoxResult.OK == result)
                {
                    // TODO: 退出
                }
            }
        }
    }
}