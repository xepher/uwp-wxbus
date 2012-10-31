using System;
using System.Net;
using org.xepher.lang;

namespace org.xepher.common
{
    public class Downloader
    {
        private static string USER_AGENT = "Mozilla/5.0 (Windows NT 6.1; rv:16.0) Gecko/20100101 Firefox/16.0 (wbs wp 1.0)";
        private static string CHINA_TELECOM = "218.90.160.85";
        private static string CHINA_MOBILE = "218.90.160.85";
        private static string CHINA_UNICOM = "221.6.99.196";
        private static string DEFAULT_PAGE = "http://{0}:9090/bustravelguide/default.aspx";
        private static string RANDOMMING_PAGE = "http://{0}:9090/bustravelguide/randomming.aspx";

        private static string GetISP(int code)
        {
            switch (code)
            {
                default:
                    return CHINA_TELECOM;
                case 1:
                    return CHINA_MOBILE;
                case 2:
                    return CHINA_UNICOM;
            }
        }

        public static void LoadRoutes(AsyncCallback asyncCallback, CookieContainer cookieContainer)
        {
            // GET /bustravelguide/ for all routes
            HttpWebRequest request =
                (HttpWebRequest)
                WebRequest.Create(string.Format(DEFAULT_PAGE, GetISP(AppSettingHelper.GetValueOrDefault("network", 0))));
            request.UserAgent = USER_AGENT;

            request.CookieContainer = cookieContainer;

            request.BeginGetResponse(asyncCallback, request);

            GlobalLoading.Instance.IsLoading = true;
        }

        public static void LoadStations(AsyncCallback asyncCallback, CookieContainer container)
        {
            // GET /bustravelguide/ for all routes
            HttpWebRequest request =
                (HttpWebRequest)
                WebRequest.Create(string.Format(DEFAULT_PAGE, GetISP(AppSettingHelper.GetValueOrDefault("network", 0))));
            request.UserAgent = USER_AGENT;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            request.CookieContainer = container;

            request.BeginGetRequestStream(asyncCallback, request);

            GlobalLoading.Instance.IsLoading = true;
        }

        public static void GetRandomming(AsyncCallback asyncCallback, CookieContainer container)
        {
            // GET randomming.aspx for Session and Cookies
            HttpWebRequest request =
                (HttpWebRequest)
                WebRequest.Create(string.Format(RANDOMMING_PAGE, GetISP(AppSettingHelper.GetValueOrDefault("network", 0))));
            request.UserAgent = USER_AGENT;

            request.CookieContainer = container;

            request.BeginGetResponse(iar => { }, request);
        }

        public static void LoadBusses(AsyncCallback asyncCallback, CookieContainer container)
        {
            // GET /bustravelguide/ for all busses
            HttpWebRequest request =
                (HttpWebRequest)
                WebRequest.Create(string.Format(DEFAULT_PAGE, GetISP(AppSettingHelper.GetValueOrDefault("network", 0))));
            request.UserAgent = USER_AGENT;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            request.CookieContainer = container;

            request.BeginGetRequestStream(asyncCallback, request);

            GlobalLoading.Instance.IsLoading = true;
        }
    }
}
