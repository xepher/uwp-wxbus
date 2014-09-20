using System;

namespace Host.Model
{
    class ExceptionMessage
    {
        public string Source { get; set; }
        public SystemInfo Info { get; set; }
        public Exception ExceptionObject { get; set; }
    }
}
