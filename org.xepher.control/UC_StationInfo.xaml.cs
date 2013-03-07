using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Tasks;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.control
{
    public partial class UC_StationInfo : UserControl
    {
        public ImageSource ImageSource
        {
            get { return imgStation.Source; }
            set { imgStation.Source = value; }
        }

        public string Text
        {
            get { return txtStation.Text; }
            set { txtStation.Text = value; }
        }

        public Grid Grid
        {
            get { return GridPanel; }
            set { GridPanel = value; }
        }

        public UC_StationInfo()
        {
            InitializeComponent();
        }

        private void ImgSMS_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(AppResource.MsgSendSMS, AppResource.TitleSendSMS, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Station station = Grid.Tag as Station;
                SmsComposeTask sms = new SmsComposeTask();
                sms.Body = string.Format("WZ{0}#{1}", station.line_name, station.station_smsid);
                sms.To = "106378909219";
                sms.Show();
            }
        }
    }
}
