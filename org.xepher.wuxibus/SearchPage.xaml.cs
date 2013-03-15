using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using org.xepher.common;
using org.xepher.control;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class SearchPage : PhoneApplicationPage
    {
        private App _app;

        public SearchPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            InitializeUIComponent();

            base.OnNavigatedTo(e);
        }

        private void InitializeUIComponent()
        {
            _app = (Application.Current as App);

            ACBLineSearch.DataContext = _app.Lines;
            ACBLineSearch.ItemsSource = _app.Lines;
            ACBLineSearch.ValueMemberPath = "line_name";
            ACBLineSearch.ItemFilter += SearchLineFilter;
            ACBLineSearch.KeyUp += ACBLineSearch_KeyUp;
        }

        private bool SearchLineFilter(string search, object item)
        {
            Line _line = item as Line;
            return _line != null && _line.line_name.Contains(search);
        }

        private void ACBLineSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToStationPage();
            }
        }

        private void BtnStationSearch_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtStationSearch.Text)) return;

            btnStationSearch.IsEnabled = false;
            SystemTray.IsVisible = true;
            GlobalLoading.Instance.IsLoading = true;

            string key = txtStationSearch.Text.Trim();

            new Thread(() =>
                {
                    List<Station> stations = _app.DAHelperInstance.GetStationByStationName(key);

                    Dispatcher.BeginInvoke(() =>
                    {
#if DEBUG
                        ToastPrompt prompt = new ToastPrompt();
                        prompt.Message = "Stations Count:" + stations.Count;
                        prompt.Show();
#endif

                        List<UC_Segment> segments = new List<UC_Segment>();
                        foreach (Station station in stations)
                        {
                            UC_Segment segment = new UC_Segment();
                            segment.Text = station.segment_name;
                            segment.Grid.Tag = new Segment()
                                {
                                    line_id = station.line_id,
                                    segment_id = station.segment_id
                                };
                            segment.Grid.Tap += Segment_OnClick;
                            segments.Add(segment);
                        }
                        SegmentList.ItemsSource = segments;

                        GlobalLoading.Instance.IsLoading = false;
                        SystemTray.IsVisible = false;
                        btnStationSearch.IsEnabled = true;
                    });
                }).Start();
        }

        private void Segment_OnClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Segment segment = (sender as Grid).Tag as Segment;
            _app.SelectedLine = _app.DAHelperInstance.GetLineById(segment.line_id);
            NavigationService.Navigate(
                new Uri(string.Format("/StationPage.xaml?segment={0}", segment.segment_id), UriKind.Relative));
        }

        private void BtnLineSearch_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToStationPage();
        }

        private void NavigateToStationPage()
        {
            Line _line = ACBLineSearch.SelectedItem as Line;
            if (_line == null)
            {
                ToastPrompt toast = new ToastPrompt();
                toast.Message = AppResource.MsgNoLine;
                toast.Show();
                ACBLineSearch.Text = string.Empty;
                return;
            }
            _app.SelectedLine = _line;
            NavigationService.Navigate(new Uri("/StationPage.xaml", UriKind.Relative));
        }
    }
}