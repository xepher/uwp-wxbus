using Framework.Serializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wuxibus.Model;

namespace wuxibus.ViewModel
{
    public class LineListViewModel
    {
        public LineListViewModel(bool isDesignView = false)
        {
            if (isDesignView)
            {
                _lines = new ObservableCollection<LineEntity>();
                using (StreamReader sr = File.OpenText("C:/Users/shaojun/Documents/GitHub/wp-wuxibus/org.xepher.kazuma/wuxibus/JsonData/1.line.json"))
                {
                    ISerializer serializer = new JsonConvertSerializer();
                    foreach (var item in serializer.Deserialize<List<LineEntity>>(sr.ReadToEnd()))
                    {
                        _lines.Add(item);
                    }
                };
            }
        }

        private IList<LineEntity> _lines;
        public IList<LineEntity> Lines
        {
            get { return _lines; }
            set { this._lines = value; }
        }
    }
}
