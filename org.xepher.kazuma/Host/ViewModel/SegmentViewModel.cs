using Framework.Container;
using Framework.Navigator;
using Framework.Serializer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Host.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;
using wuxibus.Model;

namespace Host.ViewModel
{
    public class SegmentViewModel : ViewModelBase
    {
        public const string SegmentsPropertyName = "Segments";

        private string _routeId;
        private string _segmentId;
        private string _segmentName;
        private ICommand _tapRealTimeInfoCommand;
        private IList<Station2ResultEntity> _segments;

        public string RouteId
        {
            get { return _routeId; }
            set { _routeId = value; }
        }
        public string SegmentId
        {
            get { return _segmentId; }
            set { _segmentId = value; }
        }
        public string SegmentName
        {
            get { return _segmentName; }
            set { _segmentName = value; }
        }

        public ICommand TapRealTimeInfoCommand
        {
            get
            {
                if (null == _tapRealTimeInfoCommand)
                {
                    _tapRealTimeInfoCommand = new RelayCommand<Station2Entity>(p =>
                    {
                        if (null == p) return;
                        GetRealTimeInfo(p);
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
        }

        public async Task InitSegments()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                _routeId = "7601";
                _segmentId = "76010";
                _segmentName = "760上行";

                StreamResourceInfo linesReader = Application.GetResourceStream(new Uri("/Host;component/JsonData/3.station2.json", UriKind.Relative));
                using (StreamReader sr = new StreamReader(linesReader.Stream))
                {
                    ISerializer serializer = Ioc.Container.Resolve<ISerializer>();
                    _segments = serializer.Deserialize<ObservableCollection<Station2ResultEntity>>(sr.ReadToEnd());
                };
            }
            else
            {
                string templateSegments = "http://app.wifiwx.com/bus/api.php?a=segment_station2&id={0}&nonce={1}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
                string requestUrl = SignatureUtil.GetRealRequestUrl(string.Format(templateSegments, SegmentId, SignatureUtil.RandomString()));

                Segments = await SignatureUtil.BeginWebRequest<ObservableCollection<Station2ResultEntity>>(requestUrl);
            }
        }

        public async Task GetRealTimeInfo(Station2Entity station)
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                StreamResourceInfo linesReader = Application.GetResourceStream(new Uri("/Host;component/JsonData/2.station.json", UriKind.Relative));
                using (StreamReader sr = new StreamReader(linesReader.Stream))
                {
                    ISerializer serializer = Ioc.Container.Resolve<ISerializer>();
                    StationResultEntity realTimeInfo = serializer.Deserialize<StationResultEntity>(sr.ReadToEnd());

                    foreach (var stationItem in realTimeInfo.Result)
                    {
                        foreach (var station2Item in Segments[0].List)
                        {
                            if (station2Item.StationSeq == stationItem.StationNum)
                            {
                                station2Item.ActDateTime = stationItem.ActDateTime;
                                station2Item.BusselfId = stationItem.BusselfId;
                                station2Item.BusState = stationItem.BusState;
                                station2Item.CurStopNo = stationItem.CurStopNo;
                                station2Item.LastBus = stationItem.LastBus;
                            }
                        }
                    }
                };
            }
            else
            {
                string templateRealTimeInfo = "http://app.wifiwx.com/bus/api.php?a=station_info_common&key=&nonce={0}&routeid={1}&secret=640c7088ef7811e2a4e4005056991a1f&segmentid={2}&stationseq={3}&version=0.1";
                string requestUrl = SignatureUtil.GetRealRequestUrl(string.Format(templateRealTimeInfo, SignatureUtil.RandomString(), RouteId, SegmentId, station.StationId.Length > 10 ? station.StationSeq : station.StationId));

                StationResultEntity realTimeInfo = await SignatureUtil.BeginWebRequest<StationResultEntity>(requestUrl);

                foreach (var stationItem in realTimeInfo.Result)
                {
                    foreach (var station2Item in Segments[0].List)
                    {
                        string no;
                        if (station.StationId.Length > 10)
                            no = station2Item.StationSeq;
                        else
                            no = station2Item.StationNo;
                        if (no == stationItem.StationNum)
                        {
                            station2Item.ActDateTime = stationItem.ActDateTime;
                            station2Item.BusselfId = stationItem.BusselfId;
                            station2Item.BusState = stationItem.BusState;
                            station2Item.CurStopNo = stationItem.CurStopNo;
                            station2Item.LastBus = stationItem.LastBus;
                        }
                        else
                        {
                            station2Item.ActDateTime = new DateTime();
                            station2Item.BusselfId = string.Empty;
                            station2Item.BusState = string.Empty;
                            station2Item.CurStopNo = string.Empty;
                            station2Item.LastBus = string.Empty;
                        }
                    }
                }
                
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}
