using System;
using System.Text;
using Newtonsoft.Json;
using ReactiveUI;

namespace Org.Xepher.Kazuma.Models
{
    public class StationWithRealTimeInfo : Station
    {
        [JsonProperty("routeid")]
        public string RouteId { get; set; }
        [JsonProperty("segmentid")]
        public string SegmentId { get; set; }
        [JsonProperty("stationseq")]
        public string StationSeq { get; set; }

        #region RealTimeInfo
        private const string ActDateTimePropertyName = "ActDateTime";
        private const string BusselfIdPropertyName = "BusselfId";
        private const string FlagTitlePropertyName = "FlagTitle";

        private DateTime _actDateTime;
        private string _busselfId;
        private string _flagTitle;

        public DateTime ActDateTime
        {
            get
            {
                return _actDateTime;
            }
            set
            {
                _actDateTime = value;
                this.RaisePropertyChanged(ActDateTimePropertyName);
            }
        }
        public string BusselfId
        {
            get
            {
                return _busselfId;
            }
            set
            {
                _busselfId = value;
                this.RaisePropertyChanged(BusselfIdPropertyName);
            }
        }
        public string FlagTitle
        {
            get
            {
                return _flagTitle;
            }
            set
            {
                _flagTitle = value;
                this.RaisePropertyChanged(FlagTitlePropertyName);
            }
        }
        #endregion
    }
}
