using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.Models
{
    public class StationNearBy : ModelBase
    {
        [JsonProperty("routeid")]
        public string RouteId { get; set; }
        [JsonProperty("removing")]
        public double Removing { get; set; }
        [JsonProperty("segmentid")]
        public string SegmentId { get; set; }
        [JsonProperty("stationseq")]
        public string StationSeq { get; set; }
        [JsonProperty("stationname")]
        public string StationName { get; set; }
        [JsonProperty("longitude")]
        private string Longitude { get; set; }
        [JsonProperty("latitude")]
        private string Latitude { get; set; }
        public Coordinate GPS
        {
            get
            {
                return new Coordinate { Longitude = Convert.ToDouble(this.Longitude), Latitude = Convert.ToDouble(this.Latitude) };
            }
        }
        [JsonProperty("blongitude")]
        private string BLongitude { get; set; }
        [JsonProperty("blatitude")]
        private string BLatitude { get; set; }
        public Coordinate BGPS
        {
            get
            {
                return new Coordinate { Longitude = Convert.ToDouble(this.BLongitude), Latitude = Convert.ToDouble(this.BLatitude) };
            }
        }
        [JsonProperty("station_flag")]
        public string StationFlag { get; set; }
        [JsonProperty("starttime")]
        public string StartTime { get; set; }
        [JsonProperty("station_f")]
        public string StationF { get; set; }
        [JsonProperty("start_station")]
        public string StartStation { get; set; }
        [JsonProperty("end_station")]
        public string EndStation { get; set; }
    }
}
