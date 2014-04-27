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
            // 1.generate request URL
            // line
            string templateLine = "http://app.wifiwx.com/bus/api.php?a=query_line&k={0}&key=&nonce={1}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
            string localObject = string.Format(templateLine, HttpUtility.UrlEncode("760", Encoding.UTF8), SignatureUtil.GenerateSeqId());
            // station
            string templateStation = "http://app.wifiwx.com/bus/api.php?a=station_info_common&key=&nonce={0}&routeid={1}&secret=640c7088ef7811e2a4e4005056991a1f&segmentid={2}&stationseq={3}&version=0.1";
            string localObject1 = string.Format(templateStation, SignatureUtil.RandomString(), 7601, 76010, 6707);
            // station2, id=segmentid
            string templateStation2 = "http://app.wifiwx.com/bus/api.php?a=segment_station2&id={0}&key=&nonce={1}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
            string localObject2 = string.Format(templateStation2, 30122901, SignatureUtil.RandomString());
            // news, id=routeid
            //string templateNews = "http://app.wifiwx.com/bus/api.php?a=get_news&id={0}&key=&nonce={1}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
            string templateNews = "http://app.wifiwx.com/bus/api.php?a=get_news&key=&nonce={0}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
            string localObject3 = string.Format(templateNews, SignatureUtil.RandomString());
            // nearData
            string templateNearData = "http://app.wifiwx.com/bus/api.php?a=get_segment&key=&lat={0}&lng={1}&nonce={2}&rad=1000.000000&secret=640c7088ef7811e2a4e4005056991a1f&type=1&version=0.1";
            string localObject4 = string.Format(templateNearData, 31.496320646912, 120.317770491124, SignatureUtil.RandomString());
            // nearStation
            string templateNearStation = "http://app.wifiwx.com/bus/api.php?a=get_station&key=&lat={0}&lng={1}&nonce={2}&rad=500.000000&secret=640c7088ef7811e2a4e4005056991a1f&type=1&version=0.1";
            string localObject5 = string.Format(templateNearStation, 31.496320646912, 120.317770491124, SignatureUtil.RandomString());
            // stationSuggestions
            string templateStationSuggestions = "http://app.wifiwx.com/bus/api.php?a=query_station2&k={0}&key=&nonce={1}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
            string localObject6 = string.Format(templateStationSuggestions, HttpUtility.UrlEncode("净慧", Encoding.UTF8), SignatureUtil.GenerateSeqId());
            // transferSuggestions
            string templateTransferSuggestions = "http://app.wifiwx.com/bus/api.php?a=query_station2&k={0}&key=&nonce={1}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
            string localObject7 = string.Format(templateTransferSuggestions, HttpUtility.UrlEncode("净慧", Encoding.UTF8), SignatureUtil.RandomString());
            // stationLine2
            string templateStationLine2 = "http://app.wifiwx.com/bus/api.php?a=station_line2&key=&nonce={0}&secret=640c7088ef7811e2a4e4005056991a1f&stationid={1}&version=0.1";
            string localObject8 = string.Format(templateStationLine2, SignatureUtil.RandomString(), HttpUtility.UrlEncode("6707", Encoding.UTF8));
            // user guide http://app.wifiwx.com/bus/ios/2-2/help.html

            String str1 = SignatureUtil.SHA1(SignatureUtil.FormatUrl(localObject2));
            String str2 = localObject2.Replace("&secret=640c7088ef7811e2a4e4005056991a1f", "") + "&signature=" + str1;
            localObject = str2;
            localObject = "http://app.wifiwx.com/bus/api.php?a=station_info_common&key=&nonce=3Qmos7kPyIuPVul&routeid=10&segmentid=30122901&stationseq=7&version=0.1&signature=7b19e8dd9ff8b5828e49a43d716366007e80b153";

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

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            using (StreamReader readStream = new StreamReader(receiveStream))
            {
                Console.WriteLine("Response stream received.");
                using (StreamWriter sw = File.CreateText("response.txt"))
                {
                    //byte[] bytes = Encoding.UTF8.GetBytes(readStream.ReadToEnd());
                    //byte[] bytes1 = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("gb2312"), bytes);
                    //sw.Write(Encoding.GetEncoding("gb2312").GetString(bytes1));
                    sw.Write(readStream.ReadToEnd());
                }
                response.Close();
            }
        }
    }
}
