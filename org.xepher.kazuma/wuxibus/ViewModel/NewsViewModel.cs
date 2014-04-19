using Framework.Serializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wuxibus.Model;

namespace wuxibus.ViewModel
{
    public class NewsViewModel
    {
        public NewsViewModel(bool isDesignView = false)
        {
            if (isDesignView)
            {
                _news = new ObservableCollection<NewsEntity>();
                using (StreamReader sr = File.OpenText("C:/Users/shaojun/Documents/GitHub/wp-wuxibus/org.xepher.kazuma/wuxibus/JsonData/4.news.json"))
                {
                    ISerializer serializer = new JsonConvertSerializer();
                    foreach (var item in serializer.Deserialize<List<NewsEntity>>(sr.ReadToEnd()))
                    {
                        _news.Add(item);
                    }
                };
            }
        }

        private IList<NewsEntity> _news;
        public IList<NewsEntity> News
        {
            get { return _news; }
            set { this._news = value; }
        }
    }
}
