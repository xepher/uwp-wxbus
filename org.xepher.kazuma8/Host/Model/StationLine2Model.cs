using System;
using System.Collections.Generic;

namespace wuxibus.Model
{
    public class StationLine2Entity
    {
        public string RouteId { get; set; }
        public string SegmentId { get; set; }
        public string SegmentName { get; set; }
        public string SegmentName2 { get; set; }
        public IList<LineStationInfo> Station { get; set; }
    }

    public class LineStationInfo
    {
        public string Blatitude { get; set; }
        public string Blongitude { get; set; }
        public string End_Station { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string RouteId { get; set; }
        public string SegmentId { get; set; }
        public string Start_Station { get; set; }
        public DateTime? StartTime { get; set; }
        public int Station_F { get; set; }
        public int Station_Flag { get; set; }
        public string StationId { get; set; }
        public string StationName { get; set; }
        public string StationSeq { get; set; }
    }
}
