using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using org.xepher.lang;

namespace org.xepher.control
{
    public partial class UC_PopupLoading : UserControl
    {
        public UC_PopupLoading()
        {
            InitializeComponent();

            txtLoading.Text = AppResource.PopupLoadingText;
        }
    }
}
