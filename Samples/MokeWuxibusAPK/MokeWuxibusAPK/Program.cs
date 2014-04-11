using System;
using System.IO;
using System.Web;
using System.Windows.Forms;
using MokeWuxibusAPK.Utils;

namespace MokeWuxibusAPK
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string localObject = "http://app.wifiwx.com/bus/api.php?a=query_line&k=" +
                                 HttpUtility.UrlEncode("11") + "&key=&nonce=" +
                                 SignatureUtil.GenerateSeqId() + "&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
            String str1 = SignatureUtil.SHA1(SignatureUtil.FormatUrl(localObject));
            String str2 = localObject.Replace("&secret=640c7088ef7811e2a4e4005056991a1f", "") + "&signature=" + str1;
            localObject = str2;

            Console.WriteLine(localObject);
            using (StreamWriter sw = File.CreateText("D:\\localObject.txt"))
            {
                sw.Write(localObject);
            }
        }
    }
}
