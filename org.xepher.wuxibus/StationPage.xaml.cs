using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

            _app = (Application.Current as App);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            InitializeUIComponent();

#if !DEBUG
            _app.AdHelperInstance.InitializeAds(LayoutRoot, pivotContainer);
#endif

            base.OnNavigatedTo(e);
        }

        private void InitializeUIComponent()
        {
            if (pivotContainer.Items.Count == 0)
            {
                // 正常访问(无参数)
                if (NavigationContext.QueryString.Count != 1)
                {
                    Line = _app.SelectedLine;
                    Segments = _app.DAHelperInstance.GetSegment(Line.line_id);
                    _app.SelectedSegment = SelectedSegment = Segments.First();
                }
                else
                {
                    if (NavigationContext.QueryString.ContainsKey("id"))
                    {
                        // 开始屏幕砖块进入(带id参数)
                        Line = _app.DAHelperInstance.GetLineById(int.Parse(NavigationContext.QueryString["id"]));
                        Segments = _app.DAHelperInstance.GetSegment(Line.line_id);
                        _app.SelectedSegment = SelectedSegment = Segments.First();
                    }
                    else
                    {
                        // 从查询站台页面进入(带segment参数)

                        // 如果存在QueryString指定Segment，就将特定segmentId的Segment移动到第一个位置上
                        int segmentId = int.Parse(NavigationContext.QueryString["segment"]);

                        Line = _app.SelectedLine = _app.DAHelperInstance.GetLineBySegmentId(segmentId);
                        Segments = _app.DAHelperInstance.GetSegment(Line.line_id);

                        Segment s = Segments.FirstOrDefault(t => t.segment_id == segmentId);
                        Segments.Remove(s);
                        Segments.Insert(0, s);

                        _app.SelectedSegment = SelectedSegment = s;
                    }
                }

                // 构造空Pivot框架
                foreach (Segment segment in Segments)
                {
                    pivotContainer.Title = Line.line_info;

                    Grid grid = new Grid();
                    LazyListBox.LazyListBox stationList = new LazyListBox.LazyListBox();
                    PivotItem pivotItem = new PivotItem();
                    grid.Children.Add(stationList);
                    pivotItem.Header = segment.segment_name;
                    pivotItem.Content = grid;
                    pivotContainer.Items.Add(pivotItem);
                }
            }
        }

        private void BindStationList(LazyListBox.LazyListBox stationList, List<Station> lstStation)
        {
            if (lstStation == null || lstStation.Count == 0) return;
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
                stationInfo.Grid.Tag = lstStation[index];

                if (index == 0)
                {
                    // TODO: 起始站台图标更换
                    stationInfo.ImageSource = new BitmapImage(new Uri("/Assets/Images/station.png", UriKind.Relative));
                }
                else if (index == lstStation.Count - 1)
                {
                    stationInfo.ImageSource = new BitmapImage(new Uri("/Assets/Images/station.png", UriKind.Relative));
                }
                else
                {
                    // TODO: 终点站台图标更换
                    stationInfo.ImageSource = new BitmapImage(new Uri("/Assets/Images/station.png", UriKind.Relative));
                }

                stationInfo.Grid.Tap += stationInfo_Tap;
                stationList.Items.Add(stationInfo);
            }
        }

        private void stationInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _app.SelectedStation = ((sender as Grid).Tag as Station);
            NavigationService.Navigate(new Uri("/BusPage.xaml", UriKind.Relative));
        }

        private void PivotContainer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LazyListBox.LazyListBox stationList =
                ((e.AddedItems[0] as PivotItem).Content as Grid).Children[0] as LazyListBox.LazyListBox;

            if (stationList.Items.Count > 0) return;

            Segment segment = Segments.First(s => s.segment_name == (e.AddedItems[0] as PivotItem).Header.ToString());

            new Thread(() =>
                {
                    List<Station> lstStation = _app.DAHelperInstance.GetStation(Line.line_id, segment.segment_id);
                    lstStation.ForEach(s => s.line_name = Line.line_name);

                    Dispatcher.BeginInvoke(() => BindStationList(stationList, lstStation));
                }).Start();
        }
    }
}