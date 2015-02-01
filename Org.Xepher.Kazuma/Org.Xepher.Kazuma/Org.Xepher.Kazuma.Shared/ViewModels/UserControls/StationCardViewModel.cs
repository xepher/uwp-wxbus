using System;
using System.Collections.Generic;
using System.Text;
using Org.Xepher.Kazuma.Models;

namespace Org.Xepher.Kazuma.ViewModels.UserControls
{
    public class StationCardViewModel
    {
        public StationCardViewModel(StationWithRealTimeInfo station)
        {
            CurrentStation = station;
        }

        public StationWithRealTimeInfo CurrentStation { get; private set; }
    }
}
