using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace Org.Xepher.Kazuma.Utils
{
    public static class GeoHelper
    {
        const double x_pi = 3.14159265358979324 * 3000.0 / 180.0;

        //将 GCJ-02 坐标转换成 BD-09 坐标
        public static BasicGeoposition bd_encrypt(BasicGeoposition gcgPosition)
        {
            double x = gcgPosition.Longitude, y = gcgPosition.Latitude;
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * x_pi);

            return new BasicGeoposition { Longitude = z * Math.Cos(theta) + 0.0065, Latitude = z * Math.Sin(theta) + 0.006 };
        }

        //将 BD-09 坐标转换成  GCJ-02坐标
        public static BasicGeoposition bd_decrypt(BasicGeoposition bdPosition)
        {
            double x = bdPosition.Longitude - 0.0065, y = bdPosition.Latitude - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);

            return new BasicGeoposition { Longitude = z * Math.Cos(theta), Latitude = z * Math.Sin(theta) };
        }
    }
}