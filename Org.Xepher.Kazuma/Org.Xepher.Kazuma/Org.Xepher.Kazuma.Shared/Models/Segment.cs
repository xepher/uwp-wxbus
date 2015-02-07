using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Org.Xepher.Kazuma.ViewModels.UserControls;

namespace Org.Xepher.Kazuma.Models
{
    public class Segment : ModelBase
    {
        [JsonProperty("list")]
        public List<StationWithRealTimeInfo> List { get; set; }
        [JsonProperty("starttime")]
        public string StartTime { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
