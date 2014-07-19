using System.Linq;
using Framework.Common;
using Framework.Serializer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
using wuxibus.Model;

namespace Host.ViewModel
{
    public class SegmentViewModel : ViewModelBase
    {
        private const string SegmentsPropertyName = "Segments";

        private ICommand _tapRealTimeInfoCommand;
        private IList<Station2ResultEntity> _segments;

        private LineEntity _selectedLineEntity;

        public LineEntity SelectedLineEntity
        {
            get
            {
                return _selectedLineEntity;
            }
        }

        public ICommand TapRealTimeInfoCommand
        {
            get
            {
                if (null == _tapRealTimeInfoCommand)
                {
                    _tapRealTimeInfoCommand = new RelayCommand<Station2Entity>(s =>
                    {
                        if (null == s) return;
                        GetRealTimeInfo(s);
                    });
                }

                return _tapRealTimeInfoCommand;
            }
        }

        public IList<Station2ResultEntity> Segments
        {
            get
            {
                return _segments;
            }
            set
            {
                if (_segments != value)
                {
                    _segments = value;
                    RaisePropertyChanged(SegmentsPropertyName);
                }
            }
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
                Messenger.Default.Register<LineEntity>(this, "Navigate", s =>
                {
                    _selectedLineEntity = s;
                    InitSegments();
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
                    SegmentId = "76010",
                    SegmentName = "760上行"
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
                GlobalLoading.Instance.IsLoading = true;

                string templateSegments = "http://app.wifiwx.com/bus/api.php?a=segment_station2&id={0}&nonce={1}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
                string requestUrl = SignatureUtil.GetRealRequestUrl(string.Format(templateSegments, _selectedLineEntity.SegmentId, SignatureUtil.RandomString()));

                Segments = await SignatureUtil.WebRequestAsync<ObservableCollection<Station2ResultEntity>>(requestUrl);

                // remove one-way line(opposite)
                if (Segments.Count == 2)
                {
                    if (Segments[0].List[0].StationSeq == Segments[1].List[0].StationSeq)
                    {
                        Segments.RemoveAt(1);
                    }
                }

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
                    StationResultEntity realTimeInfo = serializer.Deserialize<StationResultEntity>(sr.ReadToEnd());

                    Segments[0].List.ForEach(station2Item =>
                    {
                        if (!string.IsNullOrEmpty(station2Item.BusselfId))
                        {
                            station2Item.ActDateTime = new DateTime();
                            station2Item.BusselfId = string.Empty;
                            station2Item.BusState = string.Empty;
                            station2Item.CurStopNo = string.Empty;
                            station2Item.LastBus = string.Empty;
                        }
                    });
                    realTimeInfo.Result.ForEach(stationItem => Segments[0].List.ForEach(station2Item =>
                    {
                        if (station2Item.StationName == stationItem.StationName)
                        {
                            station2Item.ActDateTime = stationItem.ActDateTime;
                            station2Item.BusselfId = stationItem.BusselfId;
                            station2Item.BusState = stationItem.BusState;
                            station2Item.CurStopNo = stationItem.CurStopNo;
                            station2Item.LastBus = stationItem.LastBus;
                        }
                    }));
                }
            }
            else
            {
                GlobalLoading.Instance.IsLoading = true;
                
                string templateRealTimeInfo = "http://app.wifiwx.com/bus/api.php?a=station_info_common&key=&nonce={0}&routeid={1}&secret=640c7088ef7811e2a4e4005056991a1f&segmentid={2}&stationseq={3}&version=0.1";
                string requestUrl = SignatureUtil.GetRealRequestUrl(string.Format(templateRealTimeInfo, SignatureUtil.RandomString(), _selectedLineEntity.RouteId, _selectedLineEntity.SegmentId, station.StationId.Length > 10 ? station.StationSeq : station.StationId));

                StationResultEntity realTimeInfo = await SignatureUtil.WebRequestAsync<StationResultEntity>(requestUrl);

                if (!string.IsNullOrEmpty(realTimeInfo.Message))
                {
                    GlobalLoading.Instance.IsLoading = false;
                    MessageBox.Show(realTimeInfo.Message);
                    return;
                }

                // confirm which direction
                int indexList = 0;
                if (int.Parse(station.StationSeq) > int.Parse(
                    Segments[0].List.Find(s => int.Parse(s.StationSeq) == Segments[0].List.Count).StationSeq))
                {
                    indexList = 1;
                }

                foreach (var segment in Segments)
                {
                    segment.List.ForEach(station2Item =>
                    {
                        station2Item.ActDateTime = new DateTime();
                        station2Item.BusselfId = string.Empty;
                        station2Item.BusState = string.Empty;
                        station2Item.CurStopNo = string.Empty;
                        station2Item.LastBus = string.Empty;
                        station2Item.Flag_Title = string.Empty;
                    });
                }
                realTimeInfo.Result.ForEach(stationItem => Segments[indexList].List.ForEach(station2Item =>
                {
                    if (int.Parse(station2Item.StationSeq) == (int.Parse(station.StationSeq) - int.Parse(stationItem.StationNum)))
                    {
                        station2Item.ActDateTime = stationItem.ActDateTime;
                        station2Item.BusselfId = stationItem.BusselfId;
                        station2Item.BusState = stationItem.BusState;
                        station2Item.CurStopNo = stationItem.CurStopNo;
                        station2Item.LastBus = stationItem.LastBus;
                        station2Item.Flag_Title = stationItem.Flag_Title;
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
