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
        const bool isDesign = false;
        LineListViewModel _lineList = new LineListViewModel(isDesign);
        NewsViewModel _newsList = new NewsViewModel(isDesign);
        LineDetailViewModel _lineDetail = new LineDetailViewModel(isDesign);
        StationLine2ViewModel _stationLineInfo = new StationLine2ViewModel(isDesign);

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
