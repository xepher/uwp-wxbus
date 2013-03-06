using System.Windows.Controls;

namespace org.xepher.control
{
    public partial class UC_SplashScreen : UserControl
    {
        public string Text 
        {
            get { return txtLoading.Text; }
            set { txtLoading.Text = value; }
        }

        public UC_SplashScreen()
        {
            InitializeComponent();
        }
    }
}
