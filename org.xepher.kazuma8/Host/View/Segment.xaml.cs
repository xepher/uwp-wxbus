using System;
using Framework.Common;
using GalaSoft.MvvmLight.Messaging;
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

        private void AppButtonPin_OnClick(object sender, EventArgs e)
        {
            Messenger.Default.Send<string>("", "PinLine");
        }
    }
}