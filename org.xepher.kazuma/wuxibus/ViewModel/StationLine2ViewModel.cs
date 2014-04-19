using Framework.Serializer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wuxibus.Model;

namespace wuxibus.ViewModel
{
    public class StationLine2ViewModel
    {
        public StationLine2ViewModel(bool isDesignView = false)
        {
            if (isDesignView)
            {
                _stationLineInfo = new StationLine2Entity();
                using (StreamReader sr = File.OpenText("C:/Users/shaojun/Documents/GitHub/wp-wuxibus/org.xepher.kazuma/wuxibus/JsonData/9.stationLine2.json"))
                {
                    ISerializer serializer = new JsonConvertSerializer();
                    JObject jsonObj = serializer.Deserialize<JObject>(sr.ReadToEnd());
                    IEnumerable<JProperty> properties = jsonObj.Properties();
                    foreach (JProperty item in properties)
                    {
                        _stationLineInfo = serializer.Deserialize<StationLine2Entity>(item.Value.ToString());
                    }
                };
            }
        }

        private StationLine2Entity _stationLineInfo;

        public StationLine2Entity StationLineInfo
        {
            get { return _stationLineInfo; }
            set { this._stationLineInfo = value; }
        }
    }
}
