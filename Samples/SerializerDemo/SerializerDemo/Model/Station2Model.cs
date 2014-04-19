using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wuxibus.Model
{
    public class Station2Entity
    {
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
    }

    public class Station2ResultEntity
    {
        public List<Station2Entity> List { get; set; }
        public DateTime? StartTime { get; set; }
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
                double temp= longitude;
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
