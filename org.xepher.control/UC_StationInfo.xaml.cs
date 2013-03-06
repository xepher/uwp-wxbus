using System.Windows.Controls;
using System.Windows.Media;
using org.xepher.model;

namespace org.xepher.control
{
    public partial class UC_StationInfo : UserControl
    {
        public ImageSource ImageSource 
        {
            get { return imgStation.Source; }
            set { imgStation.Source = value; }
        }

        public string Text
        {
            get { return txtStation.Text; }
            set { txtStation.Text = value; }
        }

        public Station Station { get; set; }

        public UC_StationInfo()
        {
            InitializeComponent();
        }
    }
}
