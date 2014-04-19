using Framework.BaseClass;
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
    public class LineDetailViewModel
    {
        public LineDetailViewModel(bool isDesignView = false)
        {
            if (isDesignView)
            {
                _direction = new ObservableCollection<Station2ResultEntity>();
                using (StreamReader sr = File.OpenText("C:/Users/shaojun/Documents/GitHub/wp-wuxibus/org.xepher.kazuma/wuxibus/JsonData/3.station2.json"))
                {
                    ISerializer serializer = new JsonConvertSerializer();
                    foreach (var item in serializer.Deserialize<List<Station2ResultEntity>>(sr.ReadToEnd()))
                    {
                        _direction.Add(item);
                    }
                };
            }
        }

        private IList<Station2ResultEntity> _direction;

        public IList<Station2ResultEntity> Direction
        {
            get { return _direction; }
            set { this._direction = value; }
        }
    }
}