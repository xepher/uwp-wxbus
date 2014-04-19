using Framework.Serializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wuxibus.Model;

namespace wuxibus.ViewModel.DesignViewModel
{
    public class DesignViewModel
    {
        LineListViewModel _lineList = new LineListViewModel(true);
        NewsViewModel _newsList = new NewsViewModel(true);
        LineDetailViewModel _lineDetail = new LineDetailViewModel(true);
        StationLine2ViewModel _stationLineInfo = new StationLine2ViewModel(true);

        public LineListViewModel LineList
        {
            get
            {
                return _lineList;
            }
        }

        public NewsViewModel NewsList
        {
            get
            {
                return _newsList;
            }
        }

        public LineDetailViewModel LineDetail
        {
            get
            {
                return _lineDetail;
            }
        }

        public StationLine2ViewModel StationLineInfo
        {
            get
            {
                return _stationLineInfo;
            }
        }
    }
}
