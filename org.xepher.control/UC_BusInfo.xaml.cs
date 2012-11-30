using System.Windows.Controls;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.control
{
    public partial class UC_BusInfo : UserControl
    {
        private Bus _bus;

        public UC_BusInfo(Bus _bus)
        {
            InitializeComponent();

            this._bus = _bus;
            txtBusID.Text = AppResource.BusBusID + _bus.ID.ToString();
            txtStation.Text = AppResource.BusStation + _bus.Station.ToString();
            txtTime.Text = AppResource.BusTime + _bus.Time.ToShortTimeString();
            txtTTL.Text = AppResource.BusTTL + _bus.TTL.ToString();
        }
    }
}
