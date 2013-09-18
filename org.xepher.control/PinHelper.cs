using Microsoft.Phone.Shell;
using org.xepher.lang;
using System;
using System.Linq;
using System.Windows;

namespace org.xepher.control
{
    public class PinHelper
    {
        public static void PinToStart(string url, string text)
        {
            MessageBoxResult result = MessageBox.Show(string.Format(AppResource.MsgPinToStart, text), AppResource.TitlePinToStart, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.Cancel) return;

            //如果存在则删除，并在下面重新Pin到桌面
            ShellTile oldTile = ShellTile.ActiveTiles.FirstOrDefault(t => t.NavigationUri.ToString().Contains(url));
            if (oldTile != null)
            {
                oldTile.Delete();
            }

            //生成Tile
            StandardTileData myTile = new StandardTileData
            {
                BackgroundImage = new Uri("Background.png", UriKind.Relative),
                Title = text,
                Count = 0,
                BackTitle = AppResource.ApplicationTitle,
                BackContent = text,
                BackBackgroundImage = new Uri("Background.png", UriKind.Relative)
            };
            //固定到开始界面
            ShellTile.Create(new Uri(url, UriKind.Relative), myTile);
        }
    }
}
