using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using org.xepher.common;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 处理重新绑定ListBox数据源时会触发SelectionChanged事件
        private bool _isListBoxDataBinded = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            LoadRoutes();
        }

        private void LoadRoutes()
        {
            // GET /bustravelguide/ for all routes
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://218.90.160.85:9090/bustravelguide/");
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:16.0) Gecko/20100101 Firefox/16.0 (wbs wp 1.0)";
            request.Headers["Accept-Encoding"] = "gzip, deflate";
            request.Headers["Accept-Language"] = "zh-CN";
            request.Headers["Referer"] = "http://218.90.160.85:9090/bustravelguide/default.aspx";
            
            request.CookieContainer = (Application.Current as App).Container;

            request.BeginGetResponse(RoutesResponseCallback, request);

            GlobalLoading.Instance.IsLoading = true;
        }

        private void RoutesResponseCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);

            // GET randomming.aspx for Session and Cookies
            HttpWebRequest requestRandomming = (HttpWebRequest)WebRequest.Create("http://218.90.160.85:9090/bustravelguide/randomming.aspx");
            requestRandomming.Accept = "image/png, image/svg+xml, image/*;q=0.8, */*;q=0.5";
            requestRandomming.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:16.0) Gecko/20100101 Firefox/16.0 (wbs wp 1.0)";
            requestRandomming.CookieContainer = (Application.Current as App).Container;
            request.Headers["Accept-Encoding"] = "gzip, deflate";
            request.Headers["Accept-Language"] = "zh-CN";
            request.Headers["Referer"] = "http://218.90.160.85:9090/bustravelguide/default.aspx";

            requestRandomming.BeginGetResponse(iar => { }, requestRandomming);

            string result;

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
                Dispatcher.BeginInvoke(() =>
                                           {
                                               // viewstate save
                                               (Application.Current as App).ViewState = Common.GetViewState(result);

                                               _isListBoxDataBinded = true;

                                               // Resolve Routes
                                               // todo:比较 (Application.Current as App).Routes 与 Common.ResolveRoutes(e.Result) 是否一样
                                               // 不一样需要将Common.ResolveRoutes(e.Result)写入
                                               routesList.ItemsSource =
                                                   (Application.Current as App).Routes = Common.ResolveRoutes(result);

                                               routesList.SelectedIndex = -1;
                                               _isListBoxDataBinded = false;

                                               // todo: Async save Routes information
                                               IsolatedStorage.SaveToFile((Application.Current as App).Routes,
                                                                          "Data\\Routes.data");

                                               GlobalLoading.Instance.IsLoading = false;
                                           });
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBoxResult.Cancel == MessageBox.Show("Are you want to exit application?", "Exit Application", MessageBoxButton.OKCancel))
            {
                e.Cancel = true;
            }
        }

        private void ApplicationBarMenuItemAbout_OnClick(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void routesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isListBoxDataBinded)
            {
                (Application.Current as App).SelectedRoute = routesList.SelectedItem as Route;
                NavigationService.Navigate(new Uri("/StationsPage.xaml", UriKind.Relative));
            }
        }

        // refresh routes, download routes information
        private void ApplicationBarIconButtonRefresh_Click(object sender, EventArgs e)
        {
            if (MessageBoxResult.OK ==
                MessageBox.Show("Are you want to refresh routes?", "Refresh Routes", MessageBoxButton.OKCancel))
            {
                LoadRoutes();
            }
        }

        // navigate to search page
        private void ApplicationBarIconButtonSearch_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml?search=route", UriKind.Relative));
        }
    }
}