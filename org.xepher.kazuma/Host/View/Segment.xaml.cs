using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Host.Utils;
using wuxibus.Model;
using Framework.Navigator;
using Host.ViewModel;

namespace Host.View
{
    public partial class Segment : PhoneApplicationPage
    {
        public Segment()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SegmentViewModel segmentViewModel = ((SegmentViewModel)DataContext);
            segmentViewModel.RouteId = NavigationContext.QueryString["routeId"].ToString();
            segmentViewModel.SegmentId = NavigationContext.QueryString["segmentId"].ToString();
            segmentViewModel.SegmentName = NavigationContext.QueryString["segmentName"].ToString();

            base.OnNavigatedTo(e);

            segmentViewModel.InitSegments();
        }

        private void Switch_Click(object sender, EventArgs e)
        {
        }

        private void grdSegmentItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
        }
    }
}