using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Org.Xepher.Kazuma.ViewModels;
using ReactiveUI;
using System;
using Org.Xepher.Kazuma.Models;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;
using Windows.UI.Xaml.Controls.Maps;
using Org.Xepher.Kazuma.Utils;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI;
using Splat;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Org.Xepher.Kazuma.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RouteView : Page, IViewFor<RouteViewModel>
    {
        public RouteView()
        {
            this.InitializeComponent();

            InitializeBindingSettings();
        }

        #region BasicBinding for View and ViewModel

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (RouteViewModel)value; }
        }

        public RouteViewModel ViewModel
        {
            get { return (RouteViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(RouteViewModel), typeof(RouteView), new PropertyMetadata(null));

        private void InitializeBindingSettings()
        {
            this.Bind(ViewModel, vm => vm.Segments, v => v.Segments.ItemsSource);

            this.Bind(ViewModel, vm => vm.SelectedSegmentIndex, v => v.Segments.SelectedIndex);

            this.BindCommand(ViewModel, vm => vm.RefreshCommand, v => v.Refresh);

            this.BindCommand(ViewModel, vm => vm.SearchCommand, v => v.Search);

            this.BindCommand(ViewModel, vm => vm.PinCommand, v => v.Pin);

            Observable.FromEventPattern<MapInputEventArgs>(Map, "MapTapped")
                .Select(x => x.Sender)
                .Subscribe(s =>
                {
                    Grid rootGrid = ((s as MapControl).Parent as Grid);
                    if (rootGrid.RowDefinitions[1].Height.Value == 3)
                        rootGrid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                    else
                        rootGrid.RowDefinitions[1].Height = new GridLength(3, GridUnitType.Star);
                });

            this.WhenAnyValue(v => v.ViewModel.HostBootstrapper.MyPosition)
                .Where(x => x.Latitude != 0 && x.Longitude != 0)
                .Subscribe(async position =>
                {
                    MapIcon myposition = null;
                    foreach (MapIcon item in this.Map.MapElements)
                    {
                        if (item.Title == "Me")
                            myposition = item;
                    }

                    if (null == myposition)
                    {
                        myposition = new MapIcon()
                        {
                            Location = new Geopoint(position),
                            Title = "Me",
                            //Image = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new System.Uri("ms-appx:///Assets/pin.png")),
                            NormalizedAnchorPoint = new Windows.Foundation.Point() { X = 0.5, Y = 1 },
                            ZIndex = 900
                        };

                        this.Map.MapElements.Add(myposition);
                    }
                    else
                    {
                        myposition.Location = new Geopoint(position);
                    }

                    await this.Map.TrySetViewAsync(myposition.Location, 16, 0, 0, MapAnimationKind.Bow);
                });

            this.WhenAnyValue(v => v.Segments.SelectedItem)
                .Where(x => null != x)
                .Subscribe(async item =>
                {
                    this.Map.MapElements.Clear();

                    List<BasicGeoposition> positionList = (Segments.SelectedItem as Segment).List.Select(model => GeoHelper.bd_decrypt(new BasicGeoposition() { Latitude = model.BGPS.Latitude, Longitude = model.BGPS.Longitude })).ToList();
                    Geopath polylinePath = new Geopath(positionList);

                    // 1. add polyline(route) to map
                    MapPolyline polyline = new MapPolyline();
                    polyline.StrokeColor = Colors.Yellow;
                    polyline.StrokeThickness = 5;
                    polyline.ZIndex = 800;
                    polyline.Path = polylinePath;

                    this.Map.MapElements.Add(polyline);

                    // 2. add geopoint(station) to map
                    List<Geopoint> geopointList = positionList.Select(x => new Geopoint(x)).ToList();

                    // add pushpin to map
                    for (int i = 0; i < geopointList.Count; i++)
                    {
                        MapIcon pin = new MapIcon()
                        {
                            Location = geopointList[i],
                            Title = (Segments.SelectedItem as Segment).List[i].StationName,
                            //Image = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new System.Uri("ms-appx:///Assets/pin.png")),
                            NormalizedAnchorPoint = new Windows.Foundation.Point() { X = 0.5, Y = 1 },
                            ZIndex = 900
                        };
                        this.Map.MapElements.Add(pin);
                    }

                    await this.Map.TrySetViewAsync(geopointList.First(), 16, 0, 0, MapAnimationKind.Bow);
                });

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        #endregion

        // Because the ListView use visualization, so cannot binding dynamicly
        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StationWithRealTimeInfo selectedItem = (StationWithRealTimeInfo)((sender as ListView).SelectedItem);

            // set location to which user clicked
            var geoPosition = GeoHelper.bd_decrypt(new BasicGeoposition() { Latitude = selectedItem.BGPS.Latitude, Longitude = selectedItem.BGPS.Longitude });
            var location = new Geopoint(geoPosition);

            await this.Map.TrySetViewAsync(location, 16, 0, 0, MapAnimationKind.Bow);
        }
    }
}
