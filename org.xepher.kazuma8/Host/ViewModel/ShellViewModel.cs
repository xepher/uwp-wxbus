using Framework.Common;
using Framework.NavigationService;
using Framework.Serializer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Host.Model;
using Host.Utils;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;

namespace Host.ViewModel
{
    public class ShellViewModel : ViewModelBase
    {
        private INavigationService _navigationService;

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
            private set
            {
                Set(LinesPropertyName, ref _lines, value);
            }
        }
        public IList<NewsEntity> News
        {
            get
            {
                return _news;
            }
            private set
            {
                Set(NewsPropertyName, ref _news, value);
            }
        }

        public ShellViewModel(INavigationService navigationService)
            : this()
        {
            _navigationService = navigationService;
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
                                if (property.Name == item.RouteName)
                                {
                                    //item.LineInfo = serializer.Deserialize<StationLine2Entity>(property.Value.ToString());
                                }
                            }
                        };
                        _lines.Add(item);
                    }
                };
            }
            else
            {
                Messenger.Default.Register<string>(this, "LoadLinesAndNews", async s =>
                {
                    await InitLines();
                    await InitNews();
                });
            }
        }

        private bool CheckAnnouncementCircle(DateTime lastUpdateTime)
        {
            DateTime nextUpdateTime = lastUpdateTime.AddHours(((UpdateCircle)IsolatedStorageHelper.Settings["AnnouncementCircle"]).Hours);
            return DateTime.Now > nextUpdateTime;
        }

        private bool CheckAllLinesCircle(DateTime lastUpdateTime)
        {
            DateTime nextUpdateTime = lastUpdateTime.AddHours(((UpdateCircle)IsolatedStorageHelper.Settings["AllLinesCircle"]).Hours);
            return DateTime.Now > nextUpdateTime;
        }

        private async Task InitNews()
        {
            if (CheckAnnouncementCircle((DateTime)IsolatedStorageHelper.Settings["LastNewsUpdateTime"]))
            {
                // connect to wifiwuxi.com to retrieve all news, then save to local db
                DownloadNews();
            }
            else
            {
                try
                {
                    // load cached data
                    News = await SQLiteHelper.LoadNews();
                }
                catch (Exception)
                {
                    // download all news
                    DownloadNews();
                }
            }
        }

        private async Task InitLines()
        {
            if (CheckAllLinesCircle((DateTime)IsolatedStorageHelper.Settings["LastAllLinesUpdateTime"]))
            {
                // connect to wifiwuxi.com to retrieve all lines, then save to local db
                DownloadLines();
            }
            else
            {
                try
                {
                    // load cached data
                    Lines = await SQLiteHelper.LoadLines();
                }
                catch (Exception)
                {
                    // download all lines
                    DownloadLines();
                }
            }
        }

        private async void DownloadLines()
        {
            GlobalLoading.Instance.IsLoading = true;

            string requestUrl =
                SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_ALL_LINES,
                    Constants.SETTING_USER_ID,
                    Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                    SignatureUtil.GenerateSeqId(), Constants.BUS_API_SECRET));

            Lines = await SignatureUtil.WebRequestAsync<List<LineEntity>>(requestUrl);

            // save data to sqlite
            SQLiteHelper.SaveLines(Lines);

            IsolatedStorageHelper.AddOrUpdateSettings("LastAllLinesUpdateTime", DateTime.Now);

            GlobalLoading.Instance.IsLoading = false;
        }

        private async void DownloadNews()
        {
            GlobalLoading.Instance.IsLoading = true;

            string requestUrl =
                SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_NEWS, Constants.SETTING_USER_ID,
                    Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                    SignatureUtil.GenerateSeqId(), Constants.BUS_API_SECRET));

            News = await SignatureUtil.WebRequestAsync<List<NewsEntity>>(requestUrl);

            // save data to sqlite
            SQLiteHelper.SaveNews(News);

            IsolatedStorageHelper.AddOrUpdateSettings("LastNewsUpdateTime", DateTime.Now);

            GlobalLoading.Instance.IsLoading = false;
        }

        //public override void Cleanup()
        //{
        //    Messenger.Default.Unregister(this);

        //    base.Cleanup();
        //}
    }
}