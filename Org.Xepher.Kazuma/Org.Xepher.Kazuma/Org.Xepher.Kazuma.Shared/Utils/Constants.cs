using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.Utils
{
    public class Constants
    {
        public static string SETTING_USER_ID = "null";
        public const string BUS_LAT = "";
        public const string BUS_LNG = "";
        public static string DEVICE_TOKEN = "d54a1bd8272228350b2e0193da2a40a9";
        public const string BUS_API_KEY = "5";
        public const string BUS_API_SECRET = "640c72a4e4087811e2a4ec8c32f0881a";

        public const string TEMPLATE_NEWS = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=get_news&client_id={3}&key={4}&nonce={5}&secret={6}&v=3";
        public const string TEMPLATE_LINE = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=query_line&client_id={3}&k={4}&key={5}&nonce={6}&secret={7}&v=3";
        public const string TEMPLATE_SEGMENTS =
            "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=segment_station2&client_id={3}&key={4}&nonce={5}&routeid={6}&secret={7}&v=3";
        public const string TEMPLATE_REALTIME_INFO = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=station_info_common&client_id={3}&key={4}&nonce={5}&routeid={6}&secret={7}&segmentid={8}&stationseq={9}&v=3";
        public const string TEMPLATE_ALL_LINES = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=all_line&client_id={3}&key={4}&nonce={5}&secret={6}&v=3";

        public const string SETTINGS_IS_LOCALSTORAGE_ENABLED = "IsLocalStorageEnabled";
        public const string SETTINGS_LAST_LINES_UPDATE_TIME = "LastLinesUpdateTime";
        public const string SETTINGS_LINES_UPDATE_CIRCLE = "LinesUpdateCircle";
        public const string SETTINGS_ANNOUNCEMENT_UPDATE_CIRCLE = "AnnouncementUpdateCircle";
        public const string SETTINGS_LAST_NEWS_UPDATE_TIME = "LastNewsUpdateTime";

        public const string EXCEPTION_HANDLING_MESSAGE_TITLE = "Ooooops! Houston! We need help!";
        public const string EXCEPTION_HANDLING_MESSAGE_CONTENT = "程序便当了, 点击Ok发送错误给开发者!";

        public const string MSG_NETWORK_UNAVAILABLE = "网络错误, 请确保网络通畅.";
    }
}
