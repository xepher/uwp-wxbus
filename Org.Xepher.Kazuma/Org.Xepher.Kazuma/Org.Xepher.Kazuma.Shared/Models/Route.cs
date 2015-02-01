using Newtonsoft.Json;

namespace Org.Xepher.Kazuma.Models
{
    public class Route : ModelBase
    {
        [JsonProperty("flag")]
        public string Flag { get; set; }
        [JsonProperty("routeid")]
        public string RouteId { get; set; }
        [JsonProperty("routename")]
        public string RouteName { get; set; }
    }
}
