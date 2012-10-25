using System;

namespace org.xepher.model
{
    public class Bus
    {
        public int ID { get; set; }
        public Station Station { get; set; }
        public DateTime Time { get; set; }
        public int TTL { get; set; }
    }
}
