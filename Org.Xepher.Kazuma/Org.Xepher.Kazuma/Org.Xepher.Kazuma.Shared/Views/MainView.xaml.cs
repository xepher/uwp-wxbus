using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;
using Org.Xepher.Kazuma.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Org.Xepher.Kazuma.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page, IViewFor<MainViewModel>, IEnableLogger
    {
        MapIcon myposition = null;

        public MainView()
        {
            this.InitializeComponent();

            InitializeBindingSettings();
        }

        #region BasicBinding for View and ViewModel

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel)value; }
        }

        public MainViewModel ViewModel
        {
            get { return (MainViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainViewModel), typeof(MainView), new PropertyMetadata(null));

        private void InitializeBindingSettings()
        {
            // these binding are untestable, don't put complex logic binding in here

            //RxApp.SuspensionHost.ObserveAppState<MainViewModel>().BindTo(this, x => x.ViewModel);

            this.Bind(ViewModel, vm => vm.FilterTerm, v => v.FilterTerm.Text);

            this.Bind(ViewModel, vm => vm.IsEnabled, v => v.FilterTerm.IsEnabled);

            this.Bind(ViewModel, vm => vm.Routes, v => v.Routes.ItemsSource);

            this.Bind(ViewModel, vm => vm.SegmentsNearby, v => v.SegmentsNearby.ItemsSource);

            this.Bind(ViewModel, vm => vm.SelectedRoute, v => v.Routes.SelectedItem);

            this.BindCommand(ViewModel, vm => vm.RefreshCommand, v => v.Refresh);

            this.BindCommand(ViewModel, vm => vm.LocationCommand, v => v.Location);

            this.BindCommand(ViewModel, vm => vm.NavigateSettingsCommand, v => v.Setting);

            this.WhenAnyValue(v => v.ViewModel.SourceRoutes.Count)
                .Subscribe(count => this.RouteCount.Text = string.Format("WxBus -> Current Available Routes Count: {0}", count));

            this.WhenAnyValue(v => v.ViewModel.HostBootstrapper.MyPosition)
                .Where(x => x.Latitude != 0 && x.Longitude != 0)
                .Subscribe(async position =>
                {
                    foreach (MapIcon item in this.Map.MapElements)
                    {
                        if (item.Title == "Me")
                            myposition = item;
                    }

                    if (null == myposition)
                    {
                        myposition = new MapIcon()
                        {
                            Location = new Geopoint(GeoHelper.gcj_encrypt(position)),
                            Title = "Me",
                            //Image = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromUri(new System.Uri("ms-appx:///Assets/pin.png")),
                            NormalizedAnchorPoint = new Windows.Foundation.Point() { X = 0.5, Y = 1 },
                            ZIndex = 900
                        };

                        this.Map.MapElements.Add(myposition);
                    }
                    else
                    {
                        myposition.Location = new Geopoint(GeoHelper.gcj_encrypt(position));
                    }

                    await this.Map.TrySetViewAsync(myposition.Location, 16, 0, 0, MapAnimationKind.Bow);
                });

            this.WhenAnyValue(v => v.ViewModel.IsFlipSegmentsNearbyEnabled)
                .Subscribe(x =>
                {
                    this.flipSegmentsNearby.IsEnabled = x;
                    this.flipSegmentsNearby.Visibility = x ? Visibility.Visible : Visibility.Collapsed;
                });

            Observable.FromEventPattern<SelectionChangedEventArgs>(this.SegmentsNearby, "SelectionChanged")
                .Select(x => x.Sender)
                .Subscribe(async sender =>
                {
                    this.Map.MapElements.Clear();
                    if (null != myposition)
                    {
                        this.Map.MapElements.Add(myposition);
                    }

                    foreach (StationNearBy station in ((SegmentNearby)this.SegmentsNearby.SelectedItem).Stations)
                    {
                        MapIcon pin = new MapIcon
                        {
                            Location = new Geopoint(GeoHelper.bd_decrypt(new BasicGeoposition { Latitude = station.BGPS.Latitude, Longitude = station.BGPS.Longitude })),
                            Title = station.StationName,
                            NormalizedAnchorPoint = new Windows.Foundation.Point() { X = 0.5, Y = 1 },
                            ZIndex = 900
                        };

                        this.Map.MapElements.Add(pin);
                    }

                    if (((SegmentNearby)this.SegmentsNearby.SelectedItem).Stations.Any())
                    {
                        await this.Map.TrySetViewAsync(new Geopoint(GeoHelper.bd_decrypt(new BasicGeoposition { Latitude = ((SegmentNearby)this.SegmentsNearby.SelectedItem).Stations.First().BGPS.Latitude, Longitude = ((SegmentNearby)this.SegmentsNearby.SelectedItem).Stations.First().BGPS.Longitude })),
                            16, 0, 0, MapAnimationKind.Bow);
                    }
                });

            Observable.FromEventPattern<SelectionChangedEventArgs>(this.fvMain, "SelectionChanged")
                .Select(x => x.Sender)
                .Subscribe(sender =>
                {
                    switch (this.fvMain.SelectedIndex)
                    {
                        case 0:
                            this.Refresh.Visibility = Visibility.Visible;
                            this.Location.Visibility = Visibility.Collapsed;
                            break;
                        case 1:
                            this.Refresh.Visibility = Visibility.Collapsed;
                            this.Location.Visibility = Visibility.Visible;
                            break;
                        default:
                            break;
                    }
                });

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        #endregion

        private async void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Map.MapElements.Clear();
            if (null != myposition)
            {
                this.Map.MapElements.Add(myposition);
            }

            StationNearBy station = ((StationNearBy)((Panel)(sender)).DataContext);

            Geopoint stationPoint = new Geopoint(GeoHelper.bd_decrypt(new BasicGeoposition { Latitude = station.BGPS.Latitude, Longitude = station.BGPS.Longitude }));
            MapIcon pin = new MapIcon
            {
                Location = stationPoint,
                Title = station.StationName,
                NormalizedAnchorPoint = new Windows.Foundation.Point() { X = 0.5, Y = 1 },
                ZIndex = 900
            };

            this.Map.MapElements.Add(pin);

            await this.Map.TrySetViewAsync(stationPoint, 16, 0, 0, MapAnimationKind.Bow);
        }
    }
}
