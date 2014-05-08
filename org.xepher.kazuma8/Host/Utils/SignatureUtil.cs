using Framework.Container;
using Framework.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

        public static string FormatUrl(String paramString)
        {
            return paramString.Substring(1 + paramString.IndexOf("?")).Replace("&", "").Replace("=", "").ToUpper();
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

        public static string SHA1(String paramString)
        {
            return GetHashString(paramString).ToLower();
        }

        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private static byte[] GetHash(string inputString)
        {
            using (SHA1 sha1 = new SHA1Managed())
            {
                return sha1.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            }
        }

        public static string GetRealRequestUrl(string requestUrl)
        {
            String hashedRequestUrl = SHA1(FormatUrl(requestUrl));
            return requestUrl.Replace("&secret=640c7088ef7811e2a4e4005056991a1f", "") + "&signature=" + hashedRequestUrl;
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
                HttpWebResponse response = (HttpWebResponse)await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

                using (StreamReader readStream = new StreamReader(response.GetResponseStream()))
                {
                    //ISerializer serializer = Ioc.Container.Resolve<ISerializer>();
                    ISerializer serializer = ServiceLocator.Current.GetInstance<ISerializer>();
                    return serializer.Deserialize<T>(readStream.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
