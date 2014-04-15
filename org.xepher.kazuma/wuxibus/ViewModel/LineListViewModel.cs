using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wuxibus.ViewModel
{
    public class LineListViewModel
    {
        public LineListViewModel()
        {
            for (int i = 0; i < 100; i++)
            {
                _lines.Add(string.Format("Line {0}", i));
            }
        }

        private ObservableCollection<string> _lines = new ObservableCollection<string>();
        public ObservableCollection<string> Lines
        {
            get { return _lines; }
            set { this._lines = value; }
        }
    }
}
