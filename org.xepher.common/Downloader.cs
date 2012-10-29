using System;
using System.Net;

namespace org.xepher.common
{
    public class Downloader
    {
        private static string USER_AGENT = "Mozilla/5.0 (Windows NT 6.1; rv:16.0) Gecko/20100101 Firefox/16.0 (wbs wp 1.0)";
        private static string DEFAULT_PAGE = "http://218.90.160.85:9090/bustravelguide/default.aspx";
        private static string RANDOMMING_PAGE = "http://218.90.160.85:9090/bustravelguide/randomming.aspx";

        public static void LoadRoutes(AsyncCallback asyncCallback, CookieContainer cookieContainer)
        {
            // GET /bustravelguide/ for all routes
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DEFAULT_PAGE);
            request.UserAgent = USER_AGENT;

            request.CookieContainer = cookieContainer;

            request.BeginGetResponse(asyncCallback, request);

            GlobalLoading.Instance.IsLoading = true;
        }

        public static void LoadStations(AsyncCallback asyncCallback, CookieContainer cookieContainer)
        {
            // GET /bustravelguide/ for all routes
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(DEFAULT_PAGE);
            request.UserAgent = USER_AGENT;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";

            request.CookieContainer = cookieContainer;

            request.BeginGetRequestStream(asyncCallback, request);

            GlobalLoading.Instance.IsLoading = true;
        }

        public static void GetRandomming(AsyncCallback asyncCallback, CookieContainer cookieContainer)
        {
            // GET randomming.aspx for Session and Cookies
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(RANDOMMING_PAGE);
            request.UserAgent = USER_AGENT;

            request.CookieContainer = cookieContainer;

            request.BeginGetResponse(iar => { }, request);
        }
    }
}
