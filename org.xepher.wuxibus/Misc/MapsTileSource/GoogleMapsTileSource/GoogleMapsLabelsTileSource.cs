namespace org.xepher.wuxibus.misc.MapsTileSource.GoogleMapsTileSource
{
    public class MapsLabelsTileSource : MapsTileSourceBase
    {
        public MapsLabelsTileSource()
            : base("http://mt{0}.google.com/vt/lyrs=h@128&hl=zh-cn&x={1}&y={2}&z={3}&s=")
        { }
    }
}