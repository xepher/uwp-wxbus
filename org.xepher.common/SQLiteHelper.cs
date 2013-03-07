using System;
using System.Collections.Generic;
using System.Linq;
using SQLiteClient;
using org.xepher.model;

namespace org.xepher.common
{
    public class SQLiteHelper
    {
        private const string SELECT_ALL_LINE = "SELECT line_id,line_name,line_info FROM bus_line";
        private const string SELECT_SEGMENT = "SELECT line_id,segment_id,segment_name FROM bus_segment WHERE line_id = @line_id";
        private const string SELECT_STATION = "SELECT line_id,segment_id,station_num,station_smsid,station_name FROM bus_station a JOIN (SELECT station_id,station_name FROM bus_stationinfo) b ON a.station_id == b.station_id WHERE line_id = @line_id AND segment_id = @segment_id";

        // GPS定位附近的点
        private const string SELECT_STATIONS_BY_LOCATION = "SELECT station_smsid,station_name,jd_str,wd_str FROM bus_station a JOIN bus_stationinfo b ON a.station_id == b.station_id WHERE CAST(jd_str AS REAL) BETWEEN @longitude - @longituderadius AND @longitude + @longituderadius AND CAST(wd_str AS REAL) BETWEEN @latitude - @latituderadius AND @latitude + @latituderadius";
        // 站台包含的线路
        private const string SELECT_STATION_BY_LOCATION = "SELECT c.line_id,station_smsid,station_name,jd_str,wd_str,c.line_name,c.line_info FROM bus_station a JOIN bus_stationinfo b ON a.station_id == b.station_id JOIN bus_line c ON a.line_id == c.line_id WHERE CAST(jd_str AS REAL) = @longitude AND CAST(wd_str AS REAL) = @latitude";

        public const string DATABASE_FOLDER_URI = "\\Data";
        public const string DATABASE_URI = "\\Data\\wuxitraffic.db";
        public const string DATABASE_BAK_URI = "\\Data\\wuxitraffic.db.bak";
        public const string DATABASE_ZIP_URI = "\\Data\\wuxitraffic.zip";
        private object obj = new object();
        private SQLiteConnection _sqLiteConnection = null;

        public SQLiteHelper()
        {
            _sqLiteConnection = new SQLiteConnection(DATABASE_URI);
            _sqLiteConnection.Open();
        }

        public void OpenConnection()
        {
            _sqLiteConnection.Open();
        }

        public void DisposeConnection()
        {
            _sqLiteConnection.Dispose();
        }

        public List<Line> GetAllLine()
        {
            try
            {
                SQLiteCommand cmd = _sqLiteConnection.CreateCommand(SELECT_ALL_LINE);

                var lst = cmd.ExecuteQuery<Line>().ToList();

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Segment> GetSegment(int line_id)
        {
            try
            {
                SQLiteCommand cmd =
                    _sqLiteConnection.CreateCommand(SELECT_SEGMENT.Replace("@line_id", "\'" + line_id + "\'"));

                var lst = cmd.ExecuteQuery<Segment>().ToList();

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Station> GetStation(int line_id, int segment_id)
        {
            try
            {
                SQLiteCommand cmd =
                    _sqLiteConnection.CreateCommand(SELECT_STATION.Replace("@line_id", "\'" + line_id + "\'").Replace("@segment_id", "\'" + segment_id + "\'"));

                var lst = cmd.ExecuteQuery<Station>().OrderBy(s => s.station_num).ToList();

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Station> GetStationsByLocation(double longitude, double latitude, double longituderadius, double latituderadius)
        {
            try
            {
                SQLiteCommand cmd =
                    _sqLiteConnection.CreateCommand(SELECT_STATIONS_BY_LOCATION
                                                        .Replace("@longituderadius", longituderadius.ToString())
                                                        .Replace("@latituderadius", latituderadius.ToString())
                                                        .Replace("@longitude", longitude.ToString())
                                                        .Replace("@latitude", latitude.ToString()));

                var lst = cmd.ExecuteQuery<Station>().OrderBy(s => s.station_num).ToList();

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Station> GetStationByLocation(double longitude, double latitude)
        {
            try
            {
                SQLiteCommand cmd =
                    _sqLiteConnection.CreateCommand(SELECT_STATION_BY_LOCATION
                                                        .Replace("@longitude", longitude.ToString())
                                                        .Replace("@latitude", latitude.ToString()));

                var lst = cmd.ExecuteQuery<Station>().OrderBy(s => s.station_num).ToList();

                return lst;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
