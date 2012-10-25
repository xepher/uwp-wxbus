using System.Collections.Generic;

namespace org.xepher.model
{
    public class Route
    {
        public List<Station> Stations { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
