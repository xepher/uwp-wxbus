using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using org.xepher.model;

namespace org.xepher.wuxibus.misc
{
    internal class TrafficInfoHelper
    {
        private static readonly Regex regex = new Regex(@"<(.|\n)+?>", RegexOptions.IgnoreCase);

        internal static List<TrafficInfo> ResolveReturnString(string returnStr)
        {
            string[] stringArray = returnStr.Trim().Split(new[] { "κ" }, StringSplitOptions.RemoveEmptyEntries);

            //List<TrafficInfo> lstTrafficInfo = new List<TrafficInfo>();

            //foreach (var str in stringArray)
            //{
            //    string[] arrayOfString2 = str.Split(new[] { ":;" }, StringSplitOptions.RemoveEmptyEntries);

            //    TrafficInfo info = new TrafficInfo()
            //        {
            //            Id = int.Parse(arrayOfString2[0]),
            //            Title = arrayOfString2[1],
            //            Content = HttpUtility.HtmlDecode(HttpUtility.UrlDecode(regex.Replace(arrayOfString2[2],""))).Trim(),
            //            Date = DateTime.Parse(arrayOfString2[3])
            //        };
            //    lstTrafficInfo.Add(info);
            //}

            //return lstTrafficInfo;

            return
                stringArray.Select(str => str.Split(new[] {":;"}, StringSplitOptions.RemoveEmptyEntries))
                           .Select(arrayOfString2 => new TrafficInfo()
                               {
                                   Id = int.Parse(arrayOfString2[0]),
                                   Title = arrayOfString2[1],
                                   Content =
                                       HttpUtility.HtmlDecode(HttpUtility.UrlDecode(regex.Replace(arrayOfString2[2], "")))
                                                  .Trim(),
                                   Date = DateTime.Parse(arrayOfString2[3])
                               }).ToList();
        }
    }
}
