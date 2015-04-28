using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.Models
{
    public class SegmentNearby : ModelBase
    {
        [JsonProperty("routeid")]
        public string RouteId { get; set; }
        [JsonProperty("segmentid")]
        public string SegmentId { get; set; }
        [JsonProperty("segmentname")]
        public string SegmentName { get; set; }
        [JsonProperty("segmentname2")]
        public string SegmentName2 { get; set; }
        [JsonProperty("station")]
        public List<StationNearBy> Stations { get; set; }
    }
}
