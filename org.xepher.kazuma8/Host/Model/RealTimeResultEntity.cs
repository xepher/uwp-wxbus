using System.Collections.Generic;

namespace Host.Model
{
    public class RealTimeResultEntity
    {
        public string Message { get; set; }
        public List<RealTimeEntity> Result { get; set; }
    }
}
