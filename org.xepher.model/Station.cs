namespace org.xepher.model
{
    public class Station
    {
        // bus_station
        public int _ID { get; set; }
        public int line_id { get; set; }
        public int segment_id { get; set; }
        public int station_num { get; set; }
        public string station_type { get; set; }
        public int station_id { get; set; }
        public int station_smsid { get; set; }

        // bus_stationinfo
        public string station_name { get; set; }
        public string jd_str { get; set; }
        public string wd_str { get; set; }

        // bus_line
        public string line_name { get; set; }
        public string line_info { get; set; }

        // bus_segement
        public string segment_name { get; set; }
    }
}
