using System;

namespace Framework.Tile
{
    public sealed class TileInfo
    {
        public string TileId { get; set; }
        public string ShortName { get; set; }
        public string DisplayName { get; set; }
        public string Arguments { get; set; }
        public Uri LogoUri { get; set; }
        public Uri WideLogoUri { get; set; }

        public string AppName { get; set; }
        public int? Count { get; set; }
    }
}
