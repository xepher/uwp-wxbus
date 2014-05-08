using Framework.Common;
using Framework.Serializer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Host.Utils;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;
using wuxibus.Model;

namespace Host.ViewModel
{
    public class ShellViewModel : ViewModelBase
    {
        private const string LinesPropertyName = "Lines";
        private const string NewsPropertyName = "News";

        private ICommand _navigateToSegmentCommand;
        private ICommand _tapNewsCommand;
        private IList<LineEntity> _lines;
        private IList<NewsEntity> _news;

        public ICommand NavigateToSegmentCommand
        {
            get
            {
                if (null == _navigateToSegmentCommand)
                {
                    _navigateToSegmentCommand = new RelayCommand<LineEntity>(s =>
                    {
                        if (null != s)
                        {
                            Messenger.Default.Send<LineEntity>(s, "Navigate");
                        }
                    });
                }

                return _navigateToSegmentCommand;
            }
        }
        public ICommand TapNewsCommand
        {
            get
            {
                if (null == _tapNewsCommand)
                {
                    _tapNewsCommand = new RelayCommand<NewsEntity>(s =>
                    {
                        if (null == s) return;
                        MessageBox.Show(s.Body, s.Title, MessageBoxButton.OK);
                    });
                }

                return _tapNewsCommand;
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
                    //ISerializer serializer = Ioc.Container.Resolve<ISerializer>();
                    ISerializer serializer = ServiceLocator.Current.GetInstance<ISerializer>();
                    foreach (var item in serializer.Deserialize<List<LineEntity>>(sr.ReadToEnd()))
                    {
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
            else
            {
                // BUG: DesignViewModel will cause multi register, so remove DesignViewMode from xaml in PROD
                Messenger.Default.Register<string>(this, "SearchLine", s =>
                {
                    SearchLine(s);
                });
                Messenger.Default.Register<string>(this, "LoadNews", s =>
                {
                    InitNews();
                });
            }
        }

        private async Task InitNews()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                _news = new ObservableCollection<NewsEntity>();
                StreamResourceInfo newsReader = Application.GetResourceStream(new Uri("/Host;component/JsonData/4.news.json", UriKind.Relative));
                using (StreamReader sr = new StreamReader(newsReader.Stream))
                {
                    //ISerializer serializer = Ioc.Container.Resolve<ISerializer>();
                    ISerializer serializer = ServiceLocator.Current.GetInstance<ISerializer>();
                    foreach (var item in serializer.Deserialize<List<NewsEntity>>(sr.ReadToEnd()))
                    {
                        _news.Add(item);
                    }
                };
            }
            else
            {
                if (null == News)
                {
                    GlobalLoading.Instance.IsLoading = true;

                    string templateNews =
                        "http://app.wifiwx.com/bus/api.php?a=get_news&nonce={0}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
                    string requestUrl =
                        SignatureUtil.GetRealRequestUrl(string.Format(templateNews, SignatureUtil.RandomString()));

                    News = await SignatureUtil.WebRequestAsync<List<NewsEntity>>(requestUrl);

                    GlobalLoading.Instance.IsLoading = false;
                }
            }
        }

        private async Task SearchLine(string criteria)
        {
            if (criteria == string.Empty) return;
            GlobalLoading.Instance.IsLoading = true;

            string templateLine = "http://app.wifiwx.com/bus/api.php?a=query_line&k={0}&nonce={1}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
            string requestUrl = SignatureUtil.GetRealRequestUrl(string.Format(templateLine, HttpUtility.UrlEncode(criteria), SignatureUtil.GenerateSeqId()));

            Lines = await SignatureUtil.WebRequestAsync<List<LineEntity>>(requestUrl);

            GlobalLoading.Instance.IsLoading = false;
        }

        //public override void Cleanup()
        //{
        //    Messenger.Default.Unregister(this);

        //    base.Cleanup();
        //}
    }
}