using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using org.xepher.common;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            if ((Application.Current as App).GetIsNetworkAvailable())
            {
                // start loading
                WebClient downloader = new WebClient();
                Uri uri = new Uri("http://218.90.160.85:9090/bustravelguide/", UriKind.Absolute);
                downloader.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RoutesDownloaded);
                downloader.DownloadStringAsync(uri);
                GlobalLoading.Instance.IsLoading = true;
            }
            //routesList.ItemsSource = (Application.Current as App).ReadFromFile();
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

        private void RoutesDownloaded(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Result == null || e.Error != null)
            {
                MessageBox.Show("There was an error downloading the Routes!");
            }
            else
            {
                // Resolve Routes
                routesList.ItemsSource = (Application.Current as App).Routes = ResolveRoutes(e.Result);

                // save routes to IsolateStorage
                // todo: Async Save
                //(Application.Current as App).SaveToFile((Application.Current as App).Routes);
            }
            GlobalLoading.Instance.IsLoading = false;
        }

        private List<Route> ResolveRoutes(string rawhtml)
        {
            (Application.Current as App).ViewState = Common.GetViewState(rawhtml);

            int iBegin = rawhtml.IndexOf("<select name=\"ddlRoute\" id=\"ddlRoute\">");
            int iEnd = rawhtml.IndexOf("</select>") + 9;
            string RawddlRoute = rawhtml.Substring(iBegin, iEnd - iBegin);

            string pattern = "<option value=\"([^\"]*)\">([^<]*)</option>";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            MatchCollection collection = regex.Matches(RawddlRoute);

            List<Route> routes = new List<Route>();

            int index = 0;
            foreach (Match match in collection)
            {
                index++;
                Route route = new Route()
                {
                    Name = match.Groups[2].Value,
                    Value = int.Parse(match.Groups[1].Value)
                };
                routes.Add(route);
            }

            return routes;
        }

        private void routesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Route route = new Route()
            //{
            //    Name = (routesList.SelectedItem as Route).Name,
            //    Value = (routesList.SelectedItem as Route).Value
            //};
            (Application.Current as App).SelectedRoute = routesList.SelectedItem as Route;
            NavigationService.Navigate(new Uri("/StationsPage.xaml", UriKind.Relative));
        }

        // refresh routes, download routes information
        private void ApplicationBarIconButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshRoutes();
        }

        private void RefreshRoutes()
        {
            if (MessageBoxResult.Cancel ==
                MessageBox.Show("Are you want to refresh routes?", "Refresh Routes", MessageBoxButton.OKCancel))
            {
                return;
            }

            if ((Application.Current as App).GetIsNetworkAvailable())
            {
                // start loading
                WebClient downloader = new WebClient();
                Uri uri = new Uri("http://218.90.160.85:9090/bustravelguide/", UriKind.Absolute);
                downloader.DownloadStringCompleted += new DownloadStringCompletedEventHandler(RoutesDownloaded);
                downloader.DownloadStringAsync(uri);
            }
        }

        // navigate to search page
        private void ApplicationBarIconButtonSearch_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml?search=route", UriKind.Relative));
        }
    }
}