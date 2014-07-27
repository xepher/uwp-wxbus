using SQLite;

namespace Host.Model
{
    public class NewsEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
