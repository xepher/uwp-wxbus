using Newtonsoft.Json;
using System;
using System.Text;

namespace Org.Xepher.Kazuma.Models
{
    public class Station : ModelBase
    {
        [JsonProperty("bgps")]
        public Coordinate BGPS { get; set; }
        [JsonProperty("gps")]
        public Coordinate GPS { get; set; }
        [JsonProperty("stationid")]
        public string StationId { get; set; }
        [JsonProperty("stationname")]
        public string StationName { get; set; }
        [JsonProperty("stationtypeid")]
        public string StationTypeId { get; set; }
        [JsonProperty("stationtypename")]
        public string StationTypeName { get; set; }
        [JsonProperty("stationno")]
        public string StationNo { get; set; }
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
