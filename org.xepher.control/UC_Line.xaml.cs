using System.Windows.Controls;
using System.Windows.Media;

namespace org.xepher.control
{
    public partial class UC_Line : UserControl
    {
        public ImageSource ImageSource
        {
            set { imgLine.Source = value; }
        }

        public Border Border {
            get { return this.border; }
        }

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

        public Grid Image
        {
            get { return GridImage; }
            set { GridImage = value; }
        }

        public UC_Line()
        {
            InitializeComponent();
        }
    }
}
