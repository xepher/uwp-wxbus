using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Org.Xepher.Kazuma.Utils
{
    public static class SignatureUtil
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

        public static string GetHashedString(string algorithmName, string inputString)
        {
            // Convert the message string to binary data.
            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(inputString, BinaryStringEncoding.Utf8);

            // Create a HashAlgorithmProvider object.
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(algorithmName);

            // Demonstrate how to retrieve the name of the hashing algorithm.
            String strAlgNameUsed = objAlgProv.AlgorithmName;

            // Hash the message.
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);

            // Verify that the hash length equals the length specified for the algorithm.
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }

            // Convert the hash to a string (for internal use).
            String strHashHexString = CryptographicBuffer.EncodeToHexString(buffHash);

            // Return the encoded string
            return strHashHexString;
        }

        public static string GetRealRequestUrl(string requestUrl)
        {
            String hashedRequestUrl = GetHashedString(HashAlgorithmNames.Sha1, GetHashedString(HashAlgorithmNames.Sha1, FormatUrl(requestUrl)));
            return requestUrl.Replace(string.Format("&secret={0}", Constants.BUS_API_SECRET), "") + "&signature=" + hashedRequestUrl;
        }

        public static async Task<T> WebRequestAsync<T>(string requestUrl)
        {
            Uri requestUri = new Uri(requestUrl);

            // resolve the dependency via ServiceLocator
            ISerializer serializer = new JsonConvertSerializer();//ServiceLocator.Current.GetInstance<ISerializer>();
            // this token haven't been used
            CancellationTokenSource cts = new CancellationTokenSource();
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            HttpClient httpClient = new HttpClient(filter);
            httpClient.DefaultRequestHeaders.Add("UserAgent", "Org.Xepher.Kazuma.WindowsPhone; Develop for WindowsPhone8.1;");
            httpClient.DefaultRequestHeaders.Host = new HostName("app.wifiwx.com");
            httpClient.DefaultRequestHeaders.Append("KeepAlive", "true");
            httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip");

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(requestUri).AsTask(cts.Token);
                if (response.StatusCode == Windows.Web.Http.HttpStatusCode.Ok)
                {
                    string result = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                    // return deserialized json object
                    return serializer.Deserialize<T>(result);
                }
                else
                {
                    return Activator.CreateInstance<T>();
                }
            }
            catch (WebException)
            {
                return Activator.CreateInstance<T>();
            }
            catch (JsonReaderException)
            {
                return Activator.CreateInstance<T>();
            }
            catch (Exception)
            {
                return Activator.CreateInstance<T>();
            }
        }
    }
}
