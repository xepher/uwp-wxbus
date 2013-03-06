namespace org.xepher.wuxibus.misc.MapsTileSource.NokiaMapsTileSource
{
    /// <summary>
    /// µÿ–Œ≤È—Ø ( Terrain View )
    /// </summary>
    public class MapsTerrainTileSource : MapsTileSourceBase
    {
        public MapsTerrainTileSource()
            : base("http://{0}.maptile.lbs.ovi.com/maptiler/v2/maptile/newest/terrain.day/{1}/{2}/{3}/256/png8?lg=CHI&token={4}&appId={5}")
        {
        }
    }
}