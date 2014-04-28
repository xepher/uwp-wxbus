using Framework.Container;
using Framework.Navigator;
using Framework.Serializer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Host.Commands;
using Host.Model;
using Host.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Resources;
using wuxibus.Model;

namespace Host.ViewModel
{
    /// <summary>
    /// This class contains properties that the shell View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ShellViewModel : ViewModelBase
    {
        //private readonly INavigator _navigator;

        public const string LinesPropertyName = "Lines";
        public const string NewsPropertyName = "News";

        private ICommand _navigateToSegmentCommand;
        private IList<LineEntity> _lines;
        private IList<NewsEntity> _news;

        public ICommand NavigateToSegmentCommand
        {
            get
            {
                if (null == _navigateToSegmentCommand)
                {
                    _navigateToSegmentCommand = new RelayCommand<LineEntity>(p =>
                    {
                        if (null == p) return;
                        NavigatedToSegment(p);
                    });
                }

                return _navigateToSegmentCommand;
            }
        }
        public IList<LineEntity> Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                if (_lines != value)
                {
                    _lines = value;
                    RaisePropertyChanged(LinesPropertyName);
                }
            }
        }
        public IList<NewsEntity> News
        {
            get
            {
                return _news;
            }
            set
            {
                if (_news != value)
                {
                    _news = value;
                    RaisePropertyChanged(NewsPropertyName);
                }
            }
        }

        public ShellViewModel()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                _lines = new ObservableCollection<LineEntity>();
                StreamResourceInfo linesReader = Application.GetResourceStream(new Uri("/Host;component/JsonData/1.line.json", UriKind.Relative));
                using (StreamReader sr = new StreamReader(linesReader.Stream))
                {
                    ISerializer serializer = Ioc.Container.Resolve<ISerializer>();
                    foreach (var item in serializer.Deserialize<List<LineEntity>>(sr.ReadToEnd()))
                    {
                        // retrieve line info via segmentid
                        StationLine2Entity _lineInfo = new StationLine2Entity();
                        StreamResourceInfo lineInfoReader = Application.GetResourceStream(new Uri("/Host;component/JsonData/9.stationLine2.json", UriKind.Relative));
                        using (StreamReader srLineInfo = new StreamReader(lineInfoReader.Stream))
                        {
                            JObject jsonObj = serializer.Deserialize<JObject>(srLineInfo.ReadToEnd());
                            IEnumerable<JProperty> properties = jsonObj.Properties();
                            foreach (JProperty property in properties)
                            {
                                if (property.Name == item.SegmentId)
                                {
                                    item.LineInfo = serializer.Deserialize<StationLine2Entity>(property.Value.ToString());
                                }
                            }
                        };
                        _lines.Add(item);
                    }
                };
            }

            InitNews();
        }

        private void NavigatedToSegment(LineEntity line)
        {
            Messenger.Default.Send<string>(string.Format("/View/Segment.xaml?routeId={0}&segmentId={1}&segmentName={2}", line.RouteId, line.SegmentId, line.SegmentName), "Navigate");
        }

        private async Task InitNews()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                _news = new ObservableCollection<NewsEntity>();
                StreamResourceInfo newsReader = Application.GetResourceStream(new Uri("/Host;component/JsonData/4.news.json", UriKind.Relative));
                using (StreamReader sr = new StreamReader(newsReader.Stream))
                {
                    ISerializer serializer = Ioc.Container.Resolve<ISerializer>();
                    foreach (var item in serializer.Deserialize<List<NewsEntity>>(sr.ReadToEnd()))
                    {
                        _news.Add(item);
                    }
                };
            }
            else
            {
                string templateNews = "http://app.wifiwx.com/bus/api.php?a=get_news&nonce={0}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
                string requestUrl = SignatureUtil.GetRealRequestUrl(string.Format(templateNews, SignatureUtil.RandomString()));

                News = await SignatureUtil.BeginWebRequest<List<NewsEntity>>(requestUrl);
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}