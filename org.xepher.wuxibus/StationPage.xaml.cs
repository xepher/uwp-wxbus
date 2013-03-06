using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using org.xepher.control;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class StationPage : PhoneApplicationPage
    {
        private Line Line { get; set; }
        private List<Segment> Segments { get; set; }
        private Segment SelectedSegment { get; set; }

        private App _app;

        public StationPage()
        {
            InitializeComponent();

            InitializeUIComponent();
        }

        private void InitializeUIComponent()
        {
            _app = (Application.Current as App);

            Line = _app.SelectedLine;
            Segments = _app.DAHelperInstance.GetSegment(Line.line_id);
            _app.SelectedSegment = SelectedSegment = Segments.First();

            foreach (Segment segment in Segments)
            {
                List<Station> lstStation = _app.DAHelperInstance.GetStation(Line.line_id, segment.segment_id);
                pivotContainer.Title = Line.line_info;

                Grid grid = new Grid();
                LazyListBox.LazyListBox stationList = new LazyListBox.LazyListBox();
                PivotItem pivotItem = new PivotItem();
                grid.Children.Add(stationList);
                pivotItem.Header = segment.segment_name;
                pivotItem.Content = grid;
                pivotContainer.Items.Add(pivotItem);

                BindStationList(stationList, lstStation);
            }
        }

        private void BindStationList(LazyListBox.LazyListBox stationList, List<Station> lstStation)
        {
            int splitIndex = lstStation.Min(s => s.station_num) - 1;
            if (splitIndex != 0)
            {
                foreach (Station station in lstStation)
                {
                    station.station_num -= splitIndex;
                }
            }

            stationList.Items.Clear();

            for (int index = 0; index < lstStation.Count; index++)
            {
                UC_StationInfo stationInfo = new UC_StationInfo();
                stationInfo.Text = lstStation[index].station_name;
                stationInfo.Station = lstStation[index];

                if (index == 0)
                {
                    stationInfo.ImageSource = new BitmapImage(new Uri("/Assets/Images/station.png", UriKind.Relative));
                }
                else if (index == lstStation.Count - 1)
                {
                    stationInfo.ImageSource = new BitmapImage(new Uri("/Assets/Images/station.png", UriKind.Relative));
                }
                else
                {
                    stationInfo.ImageSource = new BitmapImage(new Uri("/Assets/Images/station.png", UriKind.Relative));
                }

                stationInfo.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(stationInfo_Tap);
                stationList.Items.Add(stationInfo);
            }
        }

        private void stationInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _app.SelectedStation = (sender as UC_StationInfo).Station;
            NavigationService.Navigate(new Uri("/BusPage.xaml", UriKind.Relative));
        }
    }
}