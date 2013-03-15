using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Shell;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.control
{
    public partial class UC_Line : UserControl
    {
        public string Text
        {
            get { return txtLine.Text; }
            set { txtLine.Text = value; }
        }

        public Grid Grid
        {
            get { return GridPanel; }
            set { GridPanel = value; }
        }

        public ObservableCollection<UC_Line> ParentLineList { get; set; }

        public UC_Line()
        {
            InitializeComponent();

            //ContextMenu menu = new ContextMenu();
            //MenuItem menuItem = new MenuItem { Header = AppResource.MenuPinToStart };
            //menuItem.Click += new RoutedEventHandler(menuItem_Click);
            //menu.Items.Add(menuItem);

            //// throw ArgumentOutOfRangeException in childIndex
            //ContextMenuService.SetContextMenu(imgPin, menu);
        }

        private void menuItem_Click(object sender, RoutedEventArgs e)
        {
            // Pin to start
        }

        private void ImgFav_OnClick(object sender, RoutedEventArgs e)
        {
            //ObservableCollection<Line> collection = AppSettingHelper.GetValueOrDefault(StringConstants.FAVOURITE_LINES,
            //                                                                           new ObservableCollection<Line>());
            //Line line = this.Grid.Tag as Line;
            //// TODO:添加收藏逻辑
            //if (!collection.Contains(line))
            //{
            //    collection.Insert(0, line);
            //    ParentLineList.Add(this);
            //}
            //else
            //{
            //    collection.Remove(line);
            //    ParentLineList.Remove(this);
            //}

            //AppSettingHelper.AddOrUpdateValue(StringConstants.FAVOURITE_LINES, collection);
        }

        private void ImgPin_OnClick(object sender, RoutedEventArgs e)
        {
            Line line = (Line)Grid.Tag;
            string url = string.Format("/StationPage.xaml?id={0}", line.line_id);

            MessageBoxResult result = MessageBox.Show(string.Format(AppResource.MsgPinToStart, line.line_name), AppResource.TitlePinToStart, MessageBoxButton.OKCancel);
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
                Title = line.line_name,
                Count = 0,
                BackTitle = AppResource.ApplicationTitle,
                BackContent = line.line_name,
                BackBackgroundImage = new Uri("Background.png", UriKind.Relative)
            };
            //固定到开始界面
            ShellTile.Create(new Uri(url, UriKind.Relative), myTile);
        }
    }
}
