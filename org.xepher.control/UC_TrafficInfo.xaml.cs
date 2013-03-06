using System.Windows.Controls;
using org.xepher.model;

namespace org.xepher.control
{
    public partial class UC_TrafficInfo : UserControl
    {
        public string Text
        {
            get { return txtTrafficInfo.Text; }
            set { txtTrafficInfo.Text = value; }
        }

        public TrafficInfo TrafficInfo { get; set; }

        public UC_TrafficInfo()
        {
            InitializeComponent();
        }
    }
}
