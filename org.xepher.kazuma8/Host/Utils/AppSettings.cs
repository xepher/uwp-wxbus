using System.Collections.Generic;

namespace Host.Utils
{
    public static class AppSettings
    {
        private static IList<AnnounceUpdateCircle> _announcementCircleList;

        public static IList<AnnounceUpdateCircle> AnnouncementCircleList
        {
            get
            {
                if (null == _announcementCircleList)
                {
                    _announcementCircleList = new List<AnnounceUpdateCircle>
                    {
                        new AnnounceUpdateCircle {Circle = "3小时", Hours = 3},
                        new AnnounceUpdateCircle {Circle = "6小时", Hours = 6},
                        new AnnounceUpdateCircle {Circle = "12小时", Hours = 12},
                        new AnnounceUpdateCircle {Circle = "1天", Hours = 24},
                        new AnnounceUpdateCircle {Circle = "3天", Hours = 72},
                        new AnnounceUpdateCircle {Circle = "7天", Hours = 168}
                    };
                }
                return _announcementCircleList;
            }
        }
    }

    public class AnnounceUpdateCircle
    {
        public string Circle { get; set; }
        public int Hours { get; set; }
    }
}
