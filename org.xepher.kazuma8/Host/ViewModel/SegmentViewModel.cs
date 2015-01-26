using System.Linq;
using System.Net;
using Framework.Common;
using Framework.NavigationService;
using Framework.Serializer;
using Framework.Tile;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Host.Model;
using Host.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;
using Microsoft.Practices.ServiceLocation;

namespace Host.ViewModel
{
    public class SegmentViewModel : ViewModelBase
    {
        private const string SelectedLineEntityPropertyName = "SelectedLineEntity";

        private INavigationService _navigationService;

        private const string SegmentsPropertyName = "Segments";

        private ICommand _tapRealTimeInfoCommand;
        private ICommand _tapPinLineCommand;
        private IList<Station2ResultEntity> _segments;

        private LineEntity _selectedLineEntity;

        public LineEntity SelectedLineEntity
        {
            get
            {
                return _selectedLineEntity;
            }
            set
            {
                Set(SelectedLineEntityPropertyName, ref _selectedLineEntity, value);
            }
        }

        public ICommand TapRealTimeInfoCommand
        {
            get
            {
                if (null == _tapRealTimeInfoCommand)
                {
                    _tapRealTimeInfoCommand = new RelayCommand<Station2Entity>(async s =>
                    {
                        if (null == s) return;
                        await GetRealTimeInfo(s);
                    });
                }

                return _tapRealTimeInfoCommand;
            }
        }
        public ICommand TapPinLineCommand
        {
            get
            {
                if (null == _tapPinLineCommand)
                {
                    _tapPinLineCommand = new RelayCommand(() =>
                    {
                        if (null != _segments)
                        {
                            // pin to start
                            MessageBox.Show("Pin Segments!");
                        }
                    });
                }

                return _tapPinLineCommand;
            }
        }

        public IList<Station2ResultEntity> Segments
        {
            get
            {
                return _segments;
            }
            private set
            {
                Set(SegmentsPropertyName, ref _segments, value);
            }
        }

        public SegmentViewModel(INavigationService navigationService)
            : this()
        {
            _navigationService = navigationService;
        }

        public SegmentViewModel()
        {
            //http://msdn.microsoft.com/zh-cn/magazine/jj991977.aspx
            //http://www.cnblogs.com/valentineisme/archive/2013/05/20/3088114.html
            if (ViewModelBase.IsInDesignModeStatic)
            {
                InitSegments().ContinueWith(s => GetRealTimeInfo(null));
            }
            else
            {
                Messenger.Default.Register<LineEntity>(this, "Navigate", async s =>
                {
                    SelectedLineEntity = s;
                    Segments = null;
                    await InitSegments();
                });

                //Messenger.Default.Register<Station2Entity>(this, "AutoRetrieveRealTimeInformation", s =>
                //{
                //    if (null == s) return;
                //    GetRealTimeInfo(s);
                //});

                Messenger.Default.Register<string>(this, "PinLine", async s =>
                {
                    if (null != _segments)
                    {
                        // pin to start
                        ISecondaryPinner pinner = new SecondaryPinner();

                        TileInfo tile = new TileInfo();
                        tile.AppName = "无锡公交";
                        tile.Count = 0;
                        tile.Arguments = _selectedLineEntity.RouteId;
                        tile.DisplayName = _selectedLineEntity.RouteName;
                        tile.ShortName = _selectedLineEntity.RouteName;
                        tile.TileId = _selectedLineEntity.RouteId;
                        tile.LogoUri = new Uri("/Assets/Images/Logo210-210.png", UriKind.Relative);
                        tile.WideLogoUri = new Uri("/Assets/Images/WideLogo434-210.png", UriKind.Relative);

                        await pinner.Pin(tile);
                    }
                });
            }
        }

        private async Task InitSegments()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                _selectedLineEntity = new LineEntity
                {
                    RouteId = "7601",
                    RouteName = "760上行"
                };

                StreamResourceInfo linesReader = Application.GetResourceStream(new Uri("/Host;component/JsonData/3.station2.json", UriKind.Relative));
                using (StreamReader sr = new StreamReader(linesReader.Stream))
                {
                    //ISerializer serializer = Ioc.Container.Resolve<ISerializer>();
                    ISerializer serializer = ServiceLocator.Current.GetInstance<ISerializer>();
                    _segments = serializer.Deserialize<ObservableCollection<Station2ResultEntity>>(sr.ReadToEnd());
                };
            }
            else
            {
                if (GlobalLoading.Instance.IsLoading) return;
                GlobalLoading.Instance.IsLoading = true;

                string requestUrl =
                    SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_SEGMENTS, Constants.SETTING_USER_ID,
                        Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                        SignatureUtil.GenerateSeqId(), HttpUtility.UrlEncode(SelectedLineEntity.RouteId),
                        Constants.BUS_API_SECRET));

                ObservableCollection<Station2ResultEntity> result;
                do
                {
                    result = await SignatureUtil.WebRequestAsync<ObservableCollection<Station2ResultEntity>>(requestUrl);
                } while (result == null || result.Count == 0);

                Segments = result;

                GlobalLoading.Instance.IsLoading = false;
            }
        }

        private async Task GetRealTimeInfo(Station2Entity station)
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                StreamResourceInfo linesReader = Application.GetResourceStream(new Uri("/Host;component/JsonData/2.station.json", UriKind.Relative));
                using (StreamReader sr = new StreamReader(linesReader.Stream))
                {
                    //ISerializer serializer = Ioc.Container.Resolve<ISerializer>();
                    ISerializer serializer = ServiceLocator.Current.GetInstance<ISerializer>();
                    RealTimeResultEntity realTimeInfo = serializer.Deserialize<RealTimeResultEntity>(sr.ReadToEnd());

                    Segments[0].List.ForEach(station2Item =>
                    {
                        if (!string.IsNullOrEmpty(station2Item.BusselfId))
                        {
                            station2Item.ActDateTime = new DateTime();
                            station2Item.BusselfId = string.Empty;
                        }
                    });
                    realTimeInfo.Result.ForEach(stationItem => Segments[0].List.ForEach(station2Item =>
                    {
                        if (station2Item.StationName == stationItem.StationName)
                        {
                            station2Item.ActDateTime = stationItem.ActDateTime;
                            station2Item.BusselfId = stationItem.BusselfId;
                        }
                    }));
                }
            }
            else
            {
                // if search is in-process, stop search this time
                if (GlobalLoading.Instance.IsLoading) return;
                GlobalLoading.Instance.IsLoading = true;

                string requestUrl =
                    SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_REALTIME_INFO,
                        Constants.SETTING_USER_ID, Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN,
                        Constants.BUS_API_KEY, SignatureUtil.GenerateSeqId(),
                        HttpUtility.UrlEncode(SelectedLineEntity.RouteId), Constants.BUS_API_SECRET, station.SegmentId,
                        station.StationId.Length > 10 ? station.StationSeq : station.StationId));

                RealTimeResultEntity result;
                do
                {
                    result = await SignatureUtil.WebRequestAsync<RealTimeResultEntity>(requestUrl);
                    if (result.Result == null) break;
                } while (result.Result.Count == 0);

                RealTimeResultEntity realTimeInfo = result;

                if (!string.IsNullOrEmpty(realTimeInfo.Message))
                {
                    GlobalLoading.Instance.IsLoading = false;
                    MessageBox.Show(realTimeInfo.Message);
                    return;
                }

                // confirm which direction
                int indexList = 0;

                if (Segments.Count == 2)
                {
                    if (string.IsNullOrEmpty(realTimeInfo.Result[0].CurStopNo))
                    {
                        // for wuxibus and xihuibus
                        // circle line, with same SegmentId for two direction
                        if (Segments[0].List[0].SegmentId == Segments[1].List[0].SegmentId)
                        {
                            if (int.Parse(station.StationSeq) > int.Parse(
                                Segments[0].List.Find(s => int.Parse(s.StationSeq) == Segments[0].List.Count).StationSeq))
                            {
                                indexList = 1;
                            }
                        }
                        else
                        {
                            // two one-way lines, with different SegmentsId
                            if (station.SegmentId == Segments[1].List[0].SegmentId)
                            {
                                indexList = 1;
                            }
                        }
                    }
                    else
                    {
                        // for xinqubus
                        if (station.SegmentId == Segments[1].List[0].SegmentId)
                        {
                            indexList = 1;
                        }
                    }
                }

                // clear old data
                Segments[indexList].List.ForEach(station2Item =>
                {
                    station2Item.ActDateTime = new DateTime();
                    station2Item.BusselfId = null;
                    station2Item.FlagTitle = null;
                });

                // write realtime info
                realTimeInfo.Result.ForEach(realTimeInfoItem => Segments[indexList].List.ForEach(station2Item =>
                {
                    if (string.IsNullOrEmpty(realTimeInfoItem.CurStopNo))
                    {
                        // for wuxibus and xihuibus
                        if ((int.Parse(station2Item.StationSeq) ==
                             (int.Parse(station.StationSeq) - int.Parse(realTimeInfoItem.StationNum))))
                        {
                            station2Item.ActDateTime = realTimeInfoItem.ActDateTime;
                            station2Item.BusselfId = realTimeInfoItem.BusselfId;
                            station2Item.FlagTitle = realTimeInfoItem.Flag_Title;
                        }
                    }
                    else
                    {
                        // for xinqubus
                        if (int.Parse(station2Item.StationSeq) == int.Parse(realTimeInfoItem.CurStopNo))
                        {
                            station2Item.ActDateTime = realTimeInfoItem.ActDateTime;
                            station2Item.BusselfId = realTimeInfoItem.BusselfId;
                            station2Item.FlagTitle = realTimeInfoItem.Flag_Title;
                        }
                    }
                }));

                GlobalLoading.Instance.IsLoading = false;
            }
        }

        //public override void Cleanup()
        //{
        //    // Clean up if needed
        //    Messenger.Default.Unregister(this);

        //    base.Cleanup();
        //}
    }
}
