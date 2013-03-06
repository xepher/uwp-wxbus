using System.Windows.Controls;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.control
{
    public partial class UC_LineInfo : UserControl
    {
        public Line Line { get; set; }

        public UC_LineInfo(Station station)
        {
            InitializeComponent();

            Line = new Line()
                {
                    line_id = station.line_id,
                    line_name = station.line_name,
                    line_info = station.line_info
                };
            txtLineInfo.Text = AppResource.PopupLineText + station.line_name + " " + station.line_info;
        }
    }
}
