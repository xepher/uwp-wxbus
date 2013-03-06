using System;
using System.Windows.Controls;

namespace org.xepher.control
{
    public partial class UC_TrafficDetailInfo : UserControl
    {
        public string InfoTitle
        {
            get { return txtTitle.Text; }
            set { txtTitle.Text = value; }
        }
        public string InfoContent
        {
            get { return txtContent.Text; }
            set { txtContent.Text = value; }
        }
        public DateTime InfoDate
        {
            get { return DateTime.Parse(txtDate.Text); }
            set { txtDate.Text = value.ToShortDateString(); }
        }

        public UC_TrafficDetailInfo()
        {
            InitializeComponent();
        }
    }
}
