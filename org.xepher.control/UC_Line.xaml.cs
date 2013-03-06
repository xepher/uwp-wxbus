using System.Windows.Controls;
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

        public Line Line { get; set; }

        public UC_Line()
        {
            InitializeComponent();
        }
    }
}
