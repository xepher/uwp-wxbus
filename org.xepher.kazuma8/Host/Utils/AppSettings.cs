using System.Collections.Generic;

namespace Host.Utils
{
    public static class AppSettings
    {
        private static IList<UpdateCircle> _allLinesCircleList;
        private static IList<UpdateCircle> _announcementCircleList;

        public static IList<UpdateCircle> AllLinesCircleList
        {
            get
            {
                if (null == _allLinesCircleList)
                {
                    _allLinesCircleList = new List<UpdateCircle>
                    {
                        new UpdateCircle {Circle = "1天", Hours = 24},
                        new UpdateCircle {Circle = "3天", Hours = 72},
                        new UpdateCircle {Circle = "7天", Hours = 168}
                    };
                }
                return _allLinesCircleList;
            }
        }

        public static IList<UpdateCircle> AnnouncementCircleList
        {
            get
            {
                if (null == _announcementCircleList)
                {
                    _announcementCircleList = new List<UpdateCircle>
                    {
                        new UpdateCircle {Circle = "3小时", Hours = 3},
                        new UpdateCircle {Circle = "6小时", Hours = 6},
                        new UpdateCircle {Circle = "12小时", Hours = 12},
                        new UpdateCircle {Circle = "1天", Hours = 24},
                        new UpdateCircle {Circle = "3天", Hours = 72},
                        new UpdateCircle {Circle = "7天", Hours = 168}
                    };
                }
                return _announcementCircleList;
            }
        }
    }

    public class UpdateCircle
    {
        public string Circle { get; set; }
        public int Hours { get; set; }
    }
}
