using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MokeWuxibusAPK.Utils
{
    class SignatureUtil
    {
        public static String RandomString()
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

        public static String FormatUrl(String paramString)
        {
            return paramString.Substring(1 + paramString.IndexOf("?")).Replace("&", "").Replace("=", "").ToUpper();
        }

        public static String GenerateSeqId()
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

        public static String SHA1(String paramString)
        {
            return GetHashString(paramString).ToLower();
        }

        private static byte[] GetHash(string inputString)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
