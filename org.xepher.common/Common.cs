using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Linq;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.common
{
    public class Common
    {
        public static bool GetIsNetworkAvailable(string message)
        {
            // is there network connection available
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show(message);
                return false;
            }
            return true;
        }

        public static string GetViewState(string rawHtml)
        {
            // get VIEWSTATE
            string viewStateFlag = "id=\"__VIEWSTATE\" value=\"";
            int i = rawHtml.IndexOf(viewStateFlag) + viewStateFlag.Length;
            int j = rawHtml.IndexOf("\"", i);
            string viewState = rawHtml.Substring(i, j - i);

            return Uri.EscapeDataString(viewState);
        }

        // 解析页面获得线路信息
        public static List<Route> ResolveRoutes(string rawhtml)
        {
            int iBegin = rawhtml.IndexOf("<select name=\"ddlRoute\" onchange=\"javascript:setTimeout('__doPostBack(\\'ddlRoute\\',\\'\\')', 0)\" id=\"ddlRoute\">");
            string temphtml = rawhtml.Substring(iBegin);
            int iEnd = iBegin + temphtml.IndexOf("</select>") + 9;
            string RawddlRoute = rawhtml.Substring(iBegin, iEnd - iBegin);

            string pattern = "<option value=\"([^\"]*)\">([^<]*)</option>";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            MatchCollection collection = regex.Matches(RawddlRoute);

            List<Route> routes = new List<Route>();

            foreach (Match match in collection)
            {
                Route route = new Route()
                {
                    Name = match.Groups[2].Value,
                    Value = int.Parse(match.Groups[1].Value)
                };
                routes.Add(route);
            }

            return routes;
        }

        // 解析页面获得车站信息
        public static List<Direction> ResolveStations(string rawhtml)
        {
            List<Direction> directions = ResolveDirections(rawhtml);

            string pattern = "<span id=\"rpt_ctl([^_]*)_lblStation\">([^<]*)</span>";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            MatchCollection collection = regex.Matches(rawhtml);

            List<Station> stations = new List<Station>();

            int index = 0;
            foreach (Match match in collection)
            {
                index++;
                Station station = new Station()
                {
                    Name = match.Groups[2].Value,
                    Value = index,
                    DualserialID = int.Parse(match.Groups[1].Value),
                    SngserialID = int.Parse(match.Groups[1].Value)
                };
                stations.Add(station);
            }

            Direction result = directions.FirstOrDefault(d => d.IsSelected);
            result.Stations = stations;

            return directions;
        }

        // 解析页面获得线路方向信息
        public static List<Direction> ResolveDirections(string rawhtml)
        {
            int iBegin = rawhtml.IndexOf("<select name=\"ddlSegment\" onchange=\"javascript:setTimeout('__doPostBack(\\'ddlSegment\\',\\'\\')', 0)\" id=\"ddlSegment\">");
            string temphtml = rawhtml.Substring(iBegin);
            int iEnd = iBegin + temphtml.IndexOf("</select>") + 9;
            string RawddlSegment = rawhtml.Substring(iBegin, iEnd - iBegin);

            string pattern = "<option value=\"([^\"]*)\">([^<]*)</option>";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            MatchCollection collection = regex.Matches(RawddlSegment);

            string patternSelected = "<option selected=\"selected\" value=\"([^\"]*)\">([^<]*)</option>";
            Regex regexSelected = new Regex(patternSelected, RegexOptions.IgnoreCase);

            MatchCollection collectionSelected = regexSelected.Matches(RawddlSegment);

            List<Direction> directions = new List<Direction>();

            // add unselected direction
            foreach (Match match in collection)
            {
                Direction direction = new Direction()
                {
                    Name = match.Groups[2].Value,
                    Value = int.Parse(match.Groups[1].Value),
                };
                directions.Add(direction);
            }
            // add selected direction
            Direction directionSelected = new Direction()
            {
                Name = collectionSelected[0].Groups[2].Value,
                Value = int.Parse(collectionSelected[0].Groups[1].Value),
                IsSelected = true
            };
            directions.Add(directionSelected);

            return directions;
        }

        // 解析页面获得车辆信息
        public static List<Bus> ResolveBuses(string rawhtml,ref string gvInfo)
        {
            int iBegin = rawhtml.IndexOf("<tr class=\"td_gridView_header\">");
            if (iBegin == -1)
            {
                string patternInfo = "<span id=\"lblgvInfo\" style=\"color:Red;\">([^<]*)</span>";
                Regex regexInfo = new Regex(patternInfo, RegexOptions.IgnoreCase);
                MatchCollection collectionInfo = regexInfo.Matches(rawhtml);
                gvInfo = collectionInfo[0].Groups[1].Value;
                if (string.IsNullOrEmpty(gvInfo))
                    gvInfo = AppResource.TipEmptyInfo;
                return null;
            }
            string temphtml = rawhtml.Substring(iBegin);
            int iEnd = iBegin + temphtml.IndexOf("</table>") + 8;
            string RawBusInfo = rawhtml.Substring(iBegin, iEnd - iBegin);

            string pattern = "<td>([^<]*)</td><td>([^<]*)</td><td>([^<]*)</td><td>([^<]*)</td>";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            MatchCollection collection = regex.Matches(RawBusInfo);

            List<Bus> buses = new List<Bus>();

            foreach (Match match in collection)
            {
                Bus bus = new Bus()
                                {
                                    ID = int.Parse(match.Groups[1].Value),
                                    Station = match.Groups[2].Value,
                                    Time = Convert.ToDateTime(match.Groups[3].Value.Trim()),
                                    TTL = int.Parse(match.Groups[4].Value)
                                };
                buses.Add(bus);
            }

            return buses;
        }
    }
}
