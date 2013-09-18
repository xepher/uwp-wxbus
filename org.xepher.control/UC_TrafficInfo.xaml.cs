using System.Windows.Controls;
using org.xepher.model;

namespace org.xepher.control
{
    public partial class UC_TrafficInfo : UserControl
    {
        public Grid Root {
            get { return this.LayoutRoot; }
        }

        public Border Border {
            get { return this.border; }
        }

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
