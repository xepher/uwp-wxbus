using System;
using Microsoft.Phone.Controls.Maps;

namespace org.xepher.wuxibus.misc.MapsTileSource.GoogleMapsTileSource
{
    public abstract class MapsTileSourceBase : TileSource
    {
        protected MapsTileSourceBase(string uriFormat)
            : base(uriFormat)
        { }
        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            return new Uri(string.Format(UriFormat,
                new Random().Next() % 4, x, y, zoomLevel));
        }
    }
}
