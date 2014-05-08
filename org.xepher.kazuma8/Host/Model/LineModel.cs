namespace wuxibus.Model
{
    public class LineEntity
    {
        public int PX { get; set; }
        public string RouteId { get; set; }
        public string SegmentId { get; set; }
        public string SegmentName { get; set; }
        public string SegmentName2 { get; set; }
        public string StartTime { get; set; }
        public StationLine2Entity LineInfo { get; set; }
    }
}
