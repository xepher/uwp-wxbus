using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace wuxibus.Model
{
    public class Station2Entity : ObservableObject
    {
        private const string ActDateTimePropertyName = "ActDateTime";
        private const string BusselfIdPropertyName = "BusselfId";
        private const string BusStatePropertyName = "BusState";
        private const string CurStopNoPropertyName = "CurStopNo";
        private const string LastBusPropertyName = "LastBus";
        //private const string Flag_PicPropertyName = "Flag_Pic";
        private const string Flag_TitlePropertyName = "Flag_Title";

        private DateTime _actDateTime;
        private string _busselfId;
        private string _busState;
        private string _curStopNo;
        private string _lastBus;
        //private Uri _flag_Pic;
        private string _flag_Title;

        public Coordinate BGPS { get; set; }
        public Coordinate GPS { get; set; }
        public string RouteId { get; set; }
        public string SegementId { get; set; }
        public string StationId { get; set; }
        public string StationName { get; set; }
        public string StationNo { get; set; }
        public string StationSeq { get; set; }
        public string StationTypeId { get; set; }
        public string StationTypeName { get; set; }

        public DateTime ActDateTime
        {
            get
            {
                return _actDateTime;
            }
            set
            {
                if (value != _actDateTime)
                {
                    _actDateTime = value;
                    RaisePropertyChanged(ActDateTimePropertyName);
                }
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
                if (value != _busselfId)
                {
                    _busselfId = value;
                    RaisePropertyChanged(BusselfIdPropertyName);
                }
            }
        }
        public string BusState
        {
            get
            {
                return _busState;
            }
            set
            {
                if (value != _busState)
                {
                    _busState = value;
                    RaisePropertyChanged(BusStatePropertyName);
                }
            }
        }
        public string CurStopNo
        {
            get
            {
                return _curStopNo;
            }
            set
            {
                if (value != _curStopNo)
                {
                    _curStopNo = value;
                    RaisePropertyChanged(CurStopNoPropertyName);
                }
            }
        }
        public string LastBus
        {
            get
            {
                return _lastBus;
            }
            set 
            {
                if (value != _lastBus)
                {
                    _lastBus = value;
                    RaisePropertyChanged(LastBusPropertyName);
                }
            }
        }

        // 4G flag
        public Uri Flag_Pic { get; set; }
        public string Flag_Title
        {
            get
            {
                return _flag_Title;
            }
            set
            {
                if (value != _flag_Title)
                {
                    _flag_Title = value;
                    RaisePropertyChanged(Flag_TitlePropertyName);
                }
            }
        }
    }

    public class Station2ResultEntity
    {
        public List<Station2Entity> List { get; set; }
        public string StartTime { get; set; }
    }

    [JsonConverter(typeof(CoordinateSConverter))]
    public class Coordinate
    {
        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }
    }

    public class CoordinateSConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Coordinate);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string[] results = reader.Value.ToString().Split(',');

            double longitude = Convert.ToDouble(results[0].Trim());
            double latitude = Convert.ToDouble(results[1].Trim());

            if (longitude < latitude)
            {
                double temp = longitude;
                longitude = latitude;
                latitude = temp;
            }

            Coordinate coord = new Coordinate()
            {
                Longitude = longitude,
                Latitude = latitude
            };

            return coord;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Coordinate coord = (Coordinate)value;

            StringBuilder sb = new StringBuilder();
            sb.Append(coord.Longitude.ToString());
            sb.Append(", ");
            sb.Append(coord.Latitude.ToString());

            writer.WriteValue(sb.ToString());
        }
    }
}
