using System.Device.Location;

namespace org.xepher.wuxibus.misc
{
    internal class GeoCoordinateHelper
    {
        internal static GeoCoordinate AddOffset(GeoCoordinate geoCoordinate)
        {
            // google offset
            //return new GeoCoordinate(geoCoordinate.Latitude - 0.001893, geoCoordinate.Longitude + 0.004618);
            // nokia offset
            return new GeoCoordinate(geoCoordinate.Latitude - 0.001893, geoCoordinate.Longitude + 0.004618);
        }

        internal static GeoCoordinate MinusOffset(GeoCoordinate geoCoordinate)
        {
            // google offset
            //return new GeoCoordinate(geoCoordinate.Latitude + 0.001893, geoCoordinate.Longitude - 0.004618);
            // nokia offset
            return new GeoCoordinate(geoCoordinate.Latitude + 0.001893, geoCoordinate.Longitude - 0.004618);
        }
    }
}
