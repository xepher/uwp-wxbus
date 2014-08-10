using SQLite;

namespace Host.Model
{
    public class LineEntity
    {
        public string Flag { get; set; }
        [PrimaryKey]
        public string RouteId { get; set; }
        public string RouteName { get; set; }
    }
}
