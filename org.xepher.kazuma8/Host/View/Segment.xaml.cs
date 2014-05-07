using Framework.Common;
using Microsoft.Phone.Controls;

namespace Host.View
{
    public partial class Segment : PhoneApplicationPage
    {
        public Segment()
        {
            InitializeComponent();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = false;
            base.OnBackKeyPress(e);
        }
    }
}