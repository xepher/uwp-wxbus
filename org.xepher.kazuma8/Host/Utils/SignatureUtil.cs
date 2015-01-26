using Framework.Serializer;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Host.Utils.Cryptography;
using Microsoft.Practices.ServiceLocation;

namespace Host.Utils
{
    class SignatureUtil
    {
        public static string RandomString()
        {
            const string randomSource = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random localRandom = new Random();
            StringBuilder localStringBuffer = new StringBuilder();
            for (int i = 0; i < 15; i++)
            {
                localStringBuffer.Append(randomSource[localRandom.Next(61)]);
            }

            return localStringBuffer.ToString();
        }

        public static string FormatUrl(string paramString)
        {
            string[] resultArray = paramString.Split('?');

            resultArray[0] = resultArray[0].Replace("http://", "");
            resultArray[0] = resultArray[0].Replace(".", "");
            resultArray[0] = resultArray[0].Replace("?", "");
            resultArray[0] = resultArray[0].Replace("_", "");
            resultArray[0] = resultArray[0].Replace("-", "");
            resultArray[0] = resultArray[0].Replace("/", "");
            resultArray[0] = resultArray[0].Replace("\\", "");

            resultArray[1] = resultArray[1].Replace("&", "");
            resultArray[1] = resultArray[1].Replace("=", "");

            StringBuilder sb = new StringBuilder();
            sb.Append(resultArray[0]);
            sb.Append(resultArray[1]);
            string result = sb.ToString().ToUpper();

            string s4 = "";
            string s5 = "";
            int i = 0;
            int j = result.Length;
            do
            {
                if (i >= j)
                    return s4 + s5;
                if (i % 2 == 0)
                    s4 = s4 + result.Substring(i, 1);
                else
                    s5 = s5 + result.Substring(i, 1);
                i++;
            } while (true);
        }

        public static string GenerateSeqId()
        {
            Random localRandom = new Random((int)DateTime.Now.Ticks);
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i <= 2; i++)
            {
                sb.Append(localRandom.Next(1000000).ToString());
                while (sb.Length < 6 * i)
                    sb.Insert((i - 1) * 6, "0");
            }
            return sb.ToString();
        }

        public static string SHA1(string paramString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetSHA1Hash(paramString))
                sb.Append(b.ToString("X2"));

            return sb.ToString().ToLower();
        }

        private static byte[] GetSHA1Hash(string inputString)
        {
            using (SHA1 sha1 = new SHA1Managed())
            {
                return sha1.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            }
        }

        public static string GetMD5HashString(string inputString)
        {
            return MD5.GetMd5String(inputString);
        }

        public static string GetRealRequestUrl(string requestUrl)
        {
            String hashedRequestUrl = SHA1(SHA1(FormatUrl(requestUrl)));
            return requestUrl.Replace(string.Format("&secret={0}", Constants.BUS_API_SECRET), "") + "&signature=" + hashedRequestUrl;
        }

        public static async Task<T> WebRequestAsync<T>(string requestUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            request.Headers["Host"] = "app.wifiwx.com";
            request.Headers["KeepAlive"] = "true";
            request.Headers["Accept-Encoding"] = "gzip";
            request.UserAgent = "org.xepher.kazuma.wuxibus;Develop for WindowsPhone;";

            try
            {
                HttpWebResponse response =
                    (HttpWebResponse)
                        await
                            Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

                using (StreamReader readStream = new StreamReader(response.GetResponseStream()))
                {
                    //ISerializer serializer = Ioc.Container.Resolve<ISerializer>();
                    ISerializer serializer = ServiceLocator.Current.GetInstance<ISerializer>();
                    return serializer.Deserialize<T>(readStream.ReadToEnd());
                }
            }
            catch (WebException ex)
            {
                return Activator.CreateInstance<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
