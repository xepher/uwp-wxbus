using System;
using System.Collections.Generic;
using System.Linq;
using SQLiteClient;
using org.xepher.model;

namespace org.xepher.common
{
    public class SQLiteHelper
    {
        private const string SELECT_ALL_LINE = "SELECT _ID,line_id,line_name,line_info FROM bus_line";
        private const string SELECT_SEGMENT = "SELECT _ID,line_id,segment_id,segment_name FROM bus_segment WHERE line_id = @line_id";
        private const string SELECT_STATION = "SELECT a._ID,line_id,segment_id,station_num,station_type,b.station_id,station_smsid,station_name,jd_str,wd_str FROM bus_station a JOIN bus_stationinfo b ON a.station_id == b.station_id WHERE line_id = @line_id AND segment_id = @segment_id";

        public const string DATABASE_URI = "Data\\wuxitraffic.db";
        private object obj = new object();
        private SQLiteConnection _sqLiteConnection = null;

        public List<Line> GetAllLine()
        {
            if (_sqLiteConnection == null) _sqLiteConnection = new SQLiteConnection(DATABASE_URI);
            try
            {
                _sqLiteConnection.Open();
                SQLiteCommand cmd = _sqLiteConnection.CreateCommand(SELECT_ALL_LINE);

                var lst = cmd.ExecuteQuery<Line>().ToList();

                _sqLiteConnection.Dispose();

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Segment> GetSegment(int line_id)
        {
            if (_sqLiteConnection == null) _sqLiteConnection = new SQLiteConnection(DATABASE_URI);
            try
            {
                _sqLiteConnection.Open();
                SQLiteCommand cmd =
                    _sqLiteConnection.CreateCommand(SELECT_SEGMENT.Replace("@line_id", "\'" + line_id + "\'"));

                var lst = cmd.ExecuteQuery<Segment>().ToList();

                _sqLiteConnection.Dispose();

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Station> GetStation(int line_id, int segment_id)
        {
            if (_sqLiteConnection == null) _sqLiteConnection = new SQLiteConnection(DATABASE_URI);
            try
            {
                _sqLiteConnection.Open();
                SQLiteCommand cmd =
                    _sqLiteConnection.CreateCommand(SELECT_STATION.Replace("@line_id", "\'" + line_id + "\'").Replace("@segment_id", "\'" + segment_id + "\'"));

                var lst = cmd.ExecuteQuery<Station>().OrderBy(s => s.station_num).ToList();

                _sqLiteConnection.Dispose();

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
