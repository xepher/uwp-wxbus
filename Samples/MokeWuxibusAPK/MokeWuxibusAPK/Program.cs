using System;
using System.IO;
using System.Web;
using MokeWuxibusAPK.Utils;
using System.Net;
using System.Text;

namespace MokeWuxibusAPK
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // InitUtil.class的init方法由com.hogo.android.wuxiwireless.welcome.WelcomeActivity的init方法而来
            // DEVICE_TOKEN: 来自com.hoge.android.wuxiwireless.utils.InitUtil.initMobile_client
            // Constants.BUS_API_KEY = 5 来自 system.properties
            // Constants.BUS_API_SECRET = 640c72a4e4087811e2a4ec8c32f0881a 来自 system.properties
            // Variable.DEVICE_TOKEN ： 
            // 如果等于9774d56d682e549c(在主流厂商生产的设备上，有一个很经常的bug，就是每个设备都会产生相同的ANDROID_ID：9774d56d682e549c)，就通过md5(((TelephonyManager)paramContext.getSystemService("phone")).getDeviceId() + "无线无锡");获取token
            // 如果不等于9774d56d682e549c，就使用md5(str2 + "无线无锡");作为token

            // query_line

            // {0} = Variable.SETTING_USER_ID
            // {1} = Constants.BUS_LAT
            // {2} = Constants.BUS_LNG
            // {3} = Variable.DEVICE_TOKEN
            // {4} = URLEncoder.encode(lineid, "utf-8")
            // {5} = Constants.BUS_API_KEY
            // {6} = SignatureUtil.generateSeqId()
            // {7} = Constants.BUS_API_SECRET
            string templateLine = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=query_line&client_id={3}&k={4}&key={5}&nonce={6}&secret={7}&v=3";
            string localObject = string.Format(templateLine, "null", "", "", "d54a1bd8272228350b2e0193da2a40a9", HttpUtility.UrlEncode("11", Encoding.UTF8), "5", SignatureUtil.GenerateSeqId(), "640c72a4e4087811e2a4ec8c32f0881a");

            // all_line

            // {0} = Variable.SETTING_USER_ID
            // {1} = Constants.BUS_LAT
            // {2} = Constants.BUS_LNG
            // {3} = Variable.DEVICE_TOKEN
            // {4} = URLEncoder.encode(lineid, "utf-8")
            // {5} = Constants.BUS_API_KEY
            // {6} = SignatureUtil.generateSeqId()
            // {7} = Constants.BUS_API_SECRET
            string templateAllLine = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=all_line&client_id={3}&key={4}&nonce={5}&secret={6}&v=3";
            string localObject0 = string.Format(templateAllLine, "null", "", "", "d54a1bd8272228350b2e0193da2a40a9", "5", SignatureUtil.GenerateSeqId(), "640c72a4e4087811e2a4ec8c32f0881a");

            // segment_station2

            // {0} = Variable.SETTING_USER_ID
            // {1} = Constants.BUS_LAT
            // {2} = Constants.BUS_LNG
            // {3} = Variable.DEVICE_TOKEN
            // {4} = Constants.BUS_API_KEY
            // {5} = SignatureUtil.generateSeqId()
            // {6} = routeid
            // {7} = Constants.BUS_API_SECRET
            string templateStation = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=segment_station2&client_id={3}&key={4}&nonce={5}&routeid={6}&secret={7}&v=3";
            string localObject1 = string.Format(templateStation, "null", "", "", "d54a1bd8272228350b2e0193da2a40a9", "5", SignatureUtil.GenerateSeqId(), HttpUtility.UrlEncode("11", Encoding.UTF8), "640c72a4e4087811e2a4ec8c32f0881a");

            // get_news

            // {0} = Variable.SETTING_USER_ID
            // {1} = Constants.BUS_LAT
            // {2} = Constants.BUS_LNG
            // {3} = Variable.DEVICE_TOKEN
            // {4} = Constants.BUS_API_KEY
            // {5} = SignatureUtil.generateSeqId()
            // {6} = Constants.BUS_API_SECRET
            string templateNews = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=get_news&client_id={3}&key={4}&nonce={5}&secret={6}&v=3";
            string localObject3 = string.Format(templateNews, "null", "", "", "d54a1bd8272228350b2e0193da2a40a9", "5", SignatureUtil.GenerateSeqId(), "640c72a4e4087811e2a4ec8c32f0881a");
            
            // get_segment

            // {0} = Variable.SETTING_USER_ID
            // {1} = Constants.BUS_LAT
            // {2} = Constants.BUS_LNG
            // {3} = Variable.DEVICE_TOKEN
            // {4} = Constants.BUS_API_KEY
            // {5} = lat
            // {6} = lng
            // {7} = SignatureUtil.generateSeqId()
            // {8} = Constants.BUS_API_SECRET
            string templateNearData = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=get_segment&client_id={3}&key={4}&lat={5}&lng={6}&nonce={7}&rad=1000.000000&secret={8}&type=1&v=3";
            string localObject4 = string.Format(templateNearData, "null", "", "", "d54a1bd8272228350b2e0193da2a40a9", "5", 31.496320646912, 120.317770491124, SignatureUtil.GenerateSeqId(), "640c72a4e4087811e2a4ec8c32f0881a");

            // get_station

            // {0} = Variable.SETTING_USER_ID
            // {1} = Constants.BUS_LAT
            // {2} = Constants.BUS_LNG
            // {3} = Variable.DEVICE_TOKEN
            // {4} = Constants.BUS_API_KEY
            // {5} = lat
            // {6} = lng
            // {7} = SignatureUtil.generateSeqId()
            // {8} = Constants.BUS_API_SECRET
            string templateNearStation = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=get_station&client_id={3}&key={4}&lat={5}&lng={6}&nonce={7}&rad=500.000000&secret={8}&type=1&v=3";
            string localObject5 = string.Format(templateNearStation, "null", "", "", "d54a1bd8272228350b2e0193da2a40a9", "5", 31.496320646912, 120.317770491124, SignatureUtil.GenerateSeqId(), "640c72a4e4087811e2a4ec8c32f0881a");

            // query_station2

            // {0} = Variable.SETTING_USER_ID
            // {1} = Constants.BUS_LAT
            // {2} = Constants.BUS_LNG
            // {3} = Variable.DEVICE_TOKEN
            // {4} = URLEncoder.encode(paramString, "utf-8")
            // {5} = Constants.BUS_API_KEY
            // {6} = SignatureUtil.generateSeqId()
            // {7} = Constants.BUS_API_SECRET
            string templateStationSuggestions = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=query_station2&client_id={3}&k={4}&key={5}&nonce={6}&secret={7}&v=3";
            string localObject6 = string.Format(templateStationSuggestions, "null", "", "", "d54a1bd8272228350b2e0193da2a40a9", HttpUtility.UrlEncode("净慧", Encoding.UTF8), "5", SignatureUtil.GenerateSeqId(), "640c72a4e4087811e2a4ec8c32f0881a");
            //// query_station2
            //string templateTransferSuggestions = "http://app.wifiwx.com/bus/api.php?a=query_station2&k={0}&key=&nonce={1}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
            //string localObject7 = string.Format(templateTransferSuggestions, HttpUtility.UrlEncode("净慧", Encoding.UTF8), SignatureUtil.RandomString());
            
            // station_line2

            // {0} = Variable.SETTING_USER_ID
            // {1} = Constants.BUS_LAT
            // {2} = Constants.BUS_LNG
            // {3} = Variable.DEVICE_TOKEN
            // {4} = Constants.BUS_API_KEY
            // {5} = SignatureUtil.generateSeqId()
            // {6} = Constants.BUS_API_SECRET
            // {7} = URLEncoder.encode(stationid, "utf-8")
            string templateStationLine2 = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=station_line2&client_id={3}&key={4}&nonce={5}&secret={6}&stationid={7}&v=3";
            string localObject8 = string.Format(templateStationLine2, "null", "", "", "d54a1bd8272228350b2e0193da2a40a9", "5", SignatureUtil.GenerateSeqId(), "640c72a4e4087811e2a4ec8c32f0881a", HttpUtility.UrlEncode("stationid", Encoding.UTF8));
            
            // station_info_common

            // {0} = Variable.SETTING_USER_ID
            // {1} = Constants.BUS_LAT
            // {2} = Constants.BUS_LNG
            // {3} = Variable.DEVICE_TOKEN
            // {4} = Constants.BUS_API_KEY
            // {5} = SignatureUtil.generateSeqId()
            // {6} = routeid
            // {7} = Constants.BUS_API_SECRET
            // {8} = segmentid
            // {9} = stationseq
            string templateStationInfoCommon = "http://app.wifiwx.com/bus/api.php?_member_id={0}&_trace_lat_lng={1}_{2}_1&a=station_info_common&client_id={3}&key={4}&nonce={5}&routeid={6}&secret={7}&segmentid={8}&stationseq={9}&v=3";
            string localObject9 = string.Format(templateStationInfoCommon, "null", "", "", "d54a1bd8272228350b2e0193da2a40a9", "5", SignatureUtil.GenerateSeqId(), HttpUtility.UrlEncode("7601", Encoding.UTF8), "640c72a4e4087811e2a4ec8c32f0881a", "76010", "6695");
            
            // user guide http://app.wifiwx.com/bus/ios/2-2/help.html

            String str0 = SignatureUtil.FormatUrl(localObject0);
            String str1 = SignatureUtil.SHA1(SignatureUtil.SHA1(str0));
            String str2 = localObject0.Replace("&secret=640c72a4e4087811e2a4ec8c32f0881a", "") + "&signature=" + str1;
            localObject = str2;

            // 2.save to local file
            Console.WriteLine(localObject);
            using (StreamWriter sw = File.CreateText("localObject.txt"))
            {
                sw.Write(localObject);
            }

            // 3.begin a request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(localObject);
            //request.Proxy = new WebProxy("192.168.2.250", 8888);
            request.Host = "app.wifiwx.com";
            request.KeepAlive = true;
            request.Headers.Add("Accept-Encoding", "gzip");
            WebProxy proxy = new WebProxy("***REMOVED***", 8080);
            proxy.Credentials = new NetworkCredential("***REMOVED***", "***REMOVED***");
            //request.Proxy = proxy;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            using (StreamReader readStream = new StreamReader(receiveStream))
            {
                Console.WriteLine("Response stream received.");
                string result = readStream.ReadToEnd();
                Console.WriteLine(result);
                using (StreamWriter sw = File.CreateText("response.txt"))
                {
                    sw.Write(result);
                }
                response.Close();
            }

            Console.ReadKey();
        }
    }
}
