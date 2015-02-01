using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.Models
{
    public class RealTimeBusData
    {
        public string Message { get; set; }
        public List<RealTimeBus> Result { get; set; }
    }
}
