using System.Collections.Generic;

namespace org.xepher.model
{
    public class Direction
    {
        // 关联Route.Name
        public string Name { get; set; }
        // 对应的ddlSegment值
        public int Value { get; set; }
        // 包含的站台
        public List<Station> Stations { get; set; }
        // 是否选中
        public bool IsSelected { get; set; }
        // 站台数
        public int StationsCount { get; set; }
    }
}
