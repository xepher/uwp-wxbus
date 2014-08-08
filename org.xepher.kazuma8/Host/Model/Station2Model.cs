using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace Host.Model
{
    public class Station2Entity : ObservableObject
    {
        public Coordinate BGPS { get; set; }
        public Coordinate GPS { get; set; }
        public string RouteId { get; set; }
        public string SegmentId { get; set; }
        public string StationId { get; set; }
        public string StationName { get; set; }
        public string StationNo { get; set; }
        public string StationSeq { get; set; }
        public string StationTypeId { get; set; }
        public string StationTypeName { get; set; }

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
                Set(ActDateTimePropertyName, ref _actDateTime, value);
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
                Set(BusselfIdPropertyName, ref _busselfId, value);
            }
        }
        public string Flag_Title
        {
            get
            {
                return _flagTitle;
            }
            set
            {
                Set(FlagTitlePropertyName, ref _flagTitle, value);
            }
        }
        #endregion
    }

    public class Station2ResultEntity
    {
        public List<Station2Entity> List { get; set; }
        public string StartTime { get; set; }
        public string Title { get; set; }
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
