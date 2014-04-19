using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wuxibus.Model
{
    public class StationEntity
    {
        public DateTime ActDateTime { get; set; }
        public string BusselfId { get; set; }
        public string BusState { get; set; }
        public string CurStopNo { get; set; }
        public bool LastBus { get; set; }
        public string StationName { get; set; }
        public int StationNum { get; set; }
    }

    public class StationResultEntity
    {
        public string Message { get; set; }
        [JsonProperty("result")]
        public List<StationEntity> Stations { get; set; }
    }
}
