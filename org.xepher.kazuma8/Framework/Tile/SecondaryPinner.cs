using Microsoft.Phone.Shell;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Framework.Tile
{
    public class SecondaryPinner : ISecondaryPinner
    {
        public Task<bool> Pin(TileInfo tileInfo)
        {
            var result = false;
            if (IsPinned(tileInfo))
            {
                Unpin(tileInfo).Wait();
            }

            var tileData = new StandardTileData
            {
                Title = tileInfo.DisplayName,
                BackgroundImage = tileInfo.LogoUri,
                Count = tileInfo.Count,
                BackTitle = tileInfo.AppName,
                BackBackgroundImage = new Uri("", UriKind.Relative),
                BackContent = tileInfo.DisplayName
            };

            try
            {
                ShellTile.Create(new Uri(string.Format("/View/Shell.xaml?routeId={0}&routeName={1}", tileInfo.TileId, tileInfo.DisplayName), UriKind.Relative), tileData);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return Task.FromResult<bool>(result);
        }

        public Task<bool> Unpin(TileInfo tileInfo)
        {
            ShellTile tile = FindTile(tileInfo);
            if (tile != null)
            {
                tile.Delete();
            }

            return Task.FromResult<bool>(true);
        }

        public bool IsPinned(TileInfo tileInfo)
        {
            return FindTile(tileInfo) != null;
        }

        private ShellTile FindTile(TileInfo tileInfo)
        {
            return ShellTile.ActiveTiles.FirstOrDefault(tile => tile.NavigationUri.ToString() == string.Format("/View/Shell.xaml?routeId={0}&routeName={1}", tileInfo.TileId, tileInfo.DisplayName));
        }
    }
}
