using System;
using System.Collections.Generic;

namespace Host.Model
{
    public class StationEntity
    {
        public DateTime ActDateTime { get; set; }
        public string BusselfId { get; set; }
        public string BusState { get; set; }
        public string CurStopNo { get; set; }
        public string LastBus { get; set; }
        public string StationName { get; set; }
        public string StationNum { get; set; }

        // for wuxi bus
        public string ProductId { get; set; }

        // 4G flag
        public Uri Flag_Pic { get; set; }
        public string Flag_Title { get; set; }
    }

    public class StationResultEntity
    {
        public string Message { get; set; }
        public List<StationEntity> Result { get; set; }
    }
}
