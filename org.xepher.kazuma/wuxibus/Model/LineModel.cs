using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wuxibus.Model
{
    public class LineEntity
    {
        public int PX { get; set; }
        public string RouteId { get; set; }
        public string SegmentId { get; set; }
        public string SegmentName { get; set; }
        public string SegmentName2 { get; set; }
        public DateTime? StartTime { get; set; }
    }
}
