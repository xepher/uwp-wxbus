namespace org.xepher.wuxibus.misc.MapsTileSource.NokiaMapsTileSource
{
    /// <summary>
    /// µÿÕº≤È—Ø
    /// </summary>
    public class MapsRoadTileSource : MapsTileSourceBase
    {
        public MapsRoadTileSource()
            : base("http://{0}.maptile.lbs.ovi.com/maptiler/v2/maptile/newest/normal.day/{1}/{2}/{3}/256/png8?lg=CHI&token={4}&appId={5}")
        {
        }
    }
}