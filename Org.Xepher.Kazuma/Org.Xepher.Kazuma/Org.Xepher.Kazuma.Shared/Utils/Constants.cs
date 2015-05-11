using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.System.Profile;

namespace Org.Xepher.Kazuma.Utils
{
    public class Constants
    {
        static Constants()
        {
            DEVICE_TOKEN = GetDeviceID();
        }

        public static string SETTING_USER_ID = "";
        public static string DEVICE_TOKEN = SignatureUtil.GetHashedString(HashAlgorithmNames.Md5, "WxBus");
        public const string BUS_API_KEY = "5";
        public const string BUS_API_SECRET = "640c72a4e4087811e2a4ec8c32f0881a";

        //public const string TEMPLATE_NEWS = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=get_news&client_id={3}&key={4}&nonce={5}&secret={6}&v=3";
        public const string TEMPLATE_LINE = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=query_line&client_id={3}&k={4}&key={5}&nonce={6}&secret={7}&v=3";
        public const string TEMPLATE_SEGMENTS =
            "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=segment_station2&client_id={3}&key={4}&nonce={5}&routeid={6}&secret={7}&v=3";
        public const string TEMPLATE_REALTIME_INFO = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=station_info_common&client_id={3}&key={4}&nonce={5}&routeid={6}&secret={7}&segmentid={8}&stationseq={9}&v=3";
        public const string TEMPLATE_ALL_LINES = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=all_line&client_id={3}&key={4}&nonce={5}&secret={6}&v=3";
        public const string TEMPLATE_SEGMENTS_NEARBY = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng=__1&a=get_segment&client_id={3}&key={4}&lat={1}&lng={2}&nonce={5}&rad=1000.000000&secret={6}&type=1&v=3";

        public const string SETTINGS_IS_LOCALSTORAGE_ENABLED = "IsLocalStorageEnabled";
        public const string SETTINGS_IS_LOCATION_ENABLED = "IsLocationEnabled";
        public const string SETTINGS_MSG_TO_ENABLE_LOCATION = "MsgToEnableLocation";

        public const string MSG_NETWORK_UNAVAILABLE = "网络异常，请稍后再试";
        public const string MSG_NETWORK_UNAVAILABLE_OUT_OF_RANGE = "网络异常或当前无可用数据，请稍后再试";
        public const string MSG_NETWORK_RETRY = "获取数据失败，第{0}次尝试";
        public const string MSG_NETWORK_RETRY_OUT_OF_RANGE = "获取数据失败或当前无可用数据，第{0}次尝试";

        public const string MSG_MAP_VIEW_SET_FAILURE = "地图定位失败，请重试";
        public const string MSG_MAP_LOCATION_GET = "正在定位，请稍候。";
        public const string MSG_MAP_LOCATION_HAVENT_CHANGE = "位置未发生改变，无须重新定位。";
        public const string MSG_MAP_LOCATION_SERVICE_UNAVAILABLE = "定位服务已被关闭，请到设置中打开。";

        public const string MSGBUS_TOKEN_MESSAGEBAR = "TopBarMessage";
        public const string MSGBUS_TOKEN_MY_GEOPOSITION = "MyGeoPosition";

        public const string APPBAR_TILE_ID = "App.SecondaryTile.";

        public const string STORAGE_FILE_ROUTES = "Routes.data";
        public const string STORAGE_FILE_ROUTE = "{0}.data";

        public const string PATH_SEGMENT_MAIN = "Main";
        public const string PATH_SEGMENT_ROUTE = "Route";
        public const string PATH_SEGMENT_SETTINGS = "Settings";
        public const string PATH_SEGMENT_IAP = "IAP";

        private static string GetDeviceID()
        {
            HardwareToken token = HardwareIdentification.GetPackageSpecificToken(null);
            IBuffer hardwareId = token.Id;

            HashAlgorithmProvider hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer hashed = hasher.HashData(hardwareId);

            string hashedString = CryptographicBuffer.EncodeToHexString(hashed);
            return hashedString;
        }
    }
}
