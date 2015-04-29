using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace Org.Xepher.Kazuma.Utils
{
    public static class GeoHelper
    {
        const double pi = 3.14159265358979324;
        const double x_pi = pi * 3000.0 / 180.0;
        //
        // Krasovsky 1940
        //
        // a = 6378245.0, 1/f = 298.3
        // b = a * (1 - f)
        // ee = (a^2 - b^2) / a^2;
        const double a = 6378245.0;
        const double ee = 0.00669342162296594323;

        //将 GCJ-02 坐标转换成 BD-09 坐标(火星转百度)
        public static BasicGeoposition bd_encrypt(BasicGeoposition gcjPosition)
        {
            double x = gcjPosition.Longitude, y = gcjPosition.Latitude;
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * x_pi);

            return new BasicGeoposition { Longitude = z * Math.Cos(theta) + 0.0065, Latitude = z * Math.Sin(theta) + 0.006 };
        }

        //将 BD-09 坐标转换成  GCJ-02坐标(百度转火星)
        public static BasicGeoposition bd_decrypt(BasicGeoposition bdPosition)
        {
            double x = bdPosition.Longitude - 0.0065, y = bdPosition.Latitude - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);

            return new BasicGeoposition { Longitude = z * Math.Cos(theta), Latitude = z * Math.Sin(theta) };
        }

        //将 WGS-84 坐标转换成 GCJ-02 坐标(地球转火星)
        public static BasicGeoposition gcj_encrypt(BasicGeoposition gcjPosition)
        {
            if (outOfChina(gcjPosition))
            {
                return gcjPosition;
            }
            double dLat = transformLat(gcjPosition.Longitude - 105.0, gcjPosition.Latitude - 35.0);
            double dLon = transformLon(gcjPosition.Longitude - 105.0, gcjPosition.Latitude - 35.0);
            double radLat = gcjPosition.Latitude / 180.0 * pi;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);

            return new BasicGeoposition { Latitude = gcjPosition.Latitude + dLat, Longitude = gcjPosition.Longitude + dLon };
        }

        static bool outOfChina(BasicGeoposition geoposition)
        {
            if (geoposition.Longitude < 72.004 || geoposition.Longitude > 137.8347)
                return true;
            if (geoposition.Latitude < 0.8293 || geoposition.Latitude > 55.8271)
                return true;
            return false;
        }

        static double transformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * pi) + 40.0 * Math.Sin(y / 3.0 * pi)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * pi) + 320 * Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        static double transformLon(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * pi) + 40.0 * Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * pi) + 300.0 * Math.Sin(x / 30.0 * pi)) * 2.0 / 3.0;
            return ret;
        }
    }
}