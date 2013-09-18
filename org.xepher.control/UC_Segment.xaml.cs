using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Shell;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.control
{
    public partial class UC_Segment : UserControl
    {
        public ImageSource ImageSource
        {
            set { imgLine.Source = value; }
        }

        public Border Border
        {
            get { return this.border; }
        }

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

        public Grid Image
        {
            get { return GridImage; }
            set { GridImage = value; }
        }
        
        public UC_Segment()
        {
            InitializeComponent();
        }

        private void ImgPin_OnClick(object sender, RoutedEventArgs e)
        {
            int SegmentId = (Grid.Tag as Segment).segment_id;

            string url = string.Format("/StationPage.xaml?segment={0}", SegmentId);

            PinHelper.PinToStart(url, Text);
        }
    }
}
