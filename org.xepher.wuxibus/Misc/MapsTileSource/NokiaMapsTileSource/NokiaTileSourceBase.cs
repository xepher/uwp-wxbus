using Microsoft.Phone.Controls.Maps;

namespace org.xepher.wuxibus.misc.MapsTileSource.NokiaMapsTileSource
{
    public abstract class MapsTileSourceBase : TileSource
    {
        protected MapsTileSourceBase(string uriFormat)
            : base(uriFormat)
        {
        }

        static int _tile_count = 0;

        // Register in Nokia
        static string token = "O9RpoaI2Xt3zvpGi6K94UA";
        static string appId = "lPV2S50Wbz_9bYIcl_fz";

        public override System.Uri GetUri(int x, int y, int zoomLevel)
        {
            return new System.Uri(
                string.Format(
                    UriFormat,
                    (_tile_count++ % 4) + 1,
                    zoomLevel,
                    x,
                    y,
                    token,
                    appId));
        }
    }
}
