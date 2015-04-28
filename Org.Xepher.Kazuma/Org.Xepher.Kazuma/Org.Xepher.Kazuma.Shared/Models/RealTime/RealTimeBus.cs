using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.Models
{
    public class RealTimeBus
    {
        public DateTime ActDateTime { get; set; }
        #region for wuxi bus
        public string ProductId { get; set; }
        public string BusInfo { get; set; }
        #endregion
        public string BusselfId { get; set; }
        #region for xinqu bus
        public string BusState { get; set; }
        public string CurStopNo { get; set; }
        #endregion
        #region 4G flag
        public Uri Flag_Pic { get; set; }
        public string Flag_Title { get; set; }
        #endregion
        public string LastBus { get; set; }
        public string StationName { get; set; }
        public string StationNum { get; set; }
    }
}
