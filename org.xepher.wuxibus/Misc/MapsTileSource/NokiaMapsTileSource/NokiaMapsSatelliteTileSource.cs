namespace org.xepher.wuxibus.misc.MapsTileSource.NokiaMapsTileSource
{
    /// <summary>
    /// Œ¿–«≤È—Ø ( Satellite View )
    /// </summary>
    public class MapsSatelliteTileSource : MapsTileSourceBase
    {
        public MapsSatelliteTileSource()
            : base("http://{0}.maptile.lbs.ovi.com/maptiler/v2/maptile/newest/hybrid.day/{1}/{2}/{3}/256/png8?lg=CHI&token={4}&appId={5}")
        {
        }
    }
}