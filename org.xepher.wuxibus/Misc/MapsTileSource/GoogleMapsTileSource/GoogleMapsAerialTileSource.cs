namespace org.xepher.wuxibus.misc.MapsTileSource.GoogleMapsTileSource
{
    public class MapsAerialTileSource : MapsTileSourceBase
    {
        public MapsAerialTileSource()
            : base("http://khm{0}.google.com/kh/v=62&hl=zh-cn&x={1}&y={2}&z={3}&s=")
        { }
    }
}