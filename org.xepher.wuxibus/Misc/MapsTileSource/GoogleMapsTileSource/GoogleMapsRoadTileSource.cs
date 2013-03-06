namespace org.xepher.wuxibus.misc.MapsTileSource.GoogleMapsTileSource
{
    public class MapsRoadTileSource : MapsTileSourceBase
    {
        public MapsRoadTileSource()
            : base("http://mt{0}.google.com/vt/lyrs=m@128&hl=zh-cn&x={1}&y={2}&z={3}&s=")
        { }
    }
}