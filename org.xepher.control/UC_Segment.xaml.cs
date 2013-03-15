using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Shell;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.control
{
    public partial class UC_Segment : UserControl
    {
        public string Text
        {
            get { return txtSegment.Text; }
            set { txtSegment.Text = value; }
        }

        public Grid Grid
        {
            get { return GridPanel; }
            set { GridPanel = value; }
        }
        
        public UC_Segment()
        {
            InitializeComponent();
        }

        private void ImgPin_OnClick(object sender, RoutedEventArgs e)
        {
            int SegmentId = (Grid.Tag as Segment).segment_id;

            string url = string.Format("/StationPage.xaml?segment={0}", SegmentId);

            MessageBoxResult result = MessageBox.Show(string.Format(AppResource.MsgPinToStart, Text), AppResource.TitlePinToStart, MessageBoxButton.OKCancel);
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
                Title = Text,
                Count = 0,
                BackTitle = Text,
                BackContent = AppResource.ApplicationTitle,
                BackBackgroundImage = new Uri("Background.png", UriKind.Relative)
            };
            //固定到开始界面
            ShellTile.Create(new Uri(url, UriKind.Relative), myTile);
        }
    }
}
