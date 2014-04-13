using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public TileInfo(string tileId, string shortName,string displayName,string arguments,Uri logoUri, Uri wideLogoUri, string appName, int? count)
        {
            this.TileId = tileId;
            this.ShortName = shortName;
            this.DisplayName = displayName;
            this.Arguments = arguments;
            this.LogoUri = logoUri;
            this.WideLogoUri = wideLogoUri;
            this.AppName = appName;
            this.Count = count;
        }
    }
}
