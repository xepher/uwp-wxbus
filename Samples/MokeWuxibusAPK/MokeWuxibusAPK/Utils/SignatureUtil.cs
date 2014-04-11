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
            Random localRandom = new Random();
            String str = localRandom.Next(1000000).ToString();

            while (str.Length < 6)
            {
                str = "0" + str;
            }
            return str;
        }

        public static String SHA1(String paramString)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] arrayOfByte = Encoding.UTF8.GetBytes(paramString);
            //sha.ComputeHash();
            StringBuilder localStringBuilder = new StringBuilder(arrayOfByte.Length << 1);
            for (int i = 0; i < arrayOfByte.Length; i++)
            {
                localStringBuilder.Append(string.Format("{0:X}", 0xF & arrayOfByte[i] >> 4));
                localStringBuilder.Append(string.Format("{0:X}", 0xF & arrayOfByte[i]));
            }
            return localStringBuilder.ToString();
        }
    }
}
