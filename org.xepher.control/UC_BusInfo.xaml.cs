using System.Windows.Controls;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.control
{
    public partial class UC_BusInfo : UserControl
    {
        public Border Border {
            get { return this.border; }
        }

        private BusALStationInfoCommon _bus;

        public UC_BusInfo(BusALStationInfoCommon _bus)
        {
            InitializeComponent();

            this._bus = _bus;
            txtBusID.Text = AppResource.BusBusID + _bus.busselfid.ToString();
            txtStation.Text = AppResource.BusStation + _bus.stationname;
            txtTime.Text = AppResource.BusTime + _bus.actdatetime.ToShortTimeString();
            txtTTL.Text = AppResource.BusTTL + _bus.stationnum.ToString();
        }
    }
}
