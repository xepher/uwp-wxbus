using Framework.Serializer;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wuxibus.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SerializerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ISerializer serializer = new JsonConvertSerializer();

            // 1.line.json
            //using (StreamReader sr = File.OpenText("../../SampleResults/1.line.json", FileMode.Open))
            //{
            //    List<LineEntity> lineEntities = serializer.Deserialize<List<LineEntity>>(sr.ReadToEnd());
            //}

            // 2.station.json
            //using (StreamReader sr = File.OpenText("../../SampleResults/2.station.json"))
            //{
            //    StationResultEntity stationEntities = serializer.Deserialize<StationResultEntity>(sr.ReadToEnd());
            //}

            // 3.station2.json
            //using (StreamReader sr = File.OpenText("../../SampleResults/3.station2.json"))
            //{
            //    List<Station2ResultEntity> stationEntities = serializer.Deserialize<List<Station2ResultEntity>>(sr.ReadToEnd());
            //}

            // 4.news.json
            //using (StreamReader sr = File.OpenText("../../SampleResults/4.news.json"))
            //{
            //    List<NewsEntity> newsEntities = serializer.Deserialize<List<NewsEntity>>(sr.ReadToEnd());
            //}

            // 9.stationLine2.json
            using (StreamReader sr = File.OpenText("../../SampleResults/9.stationLine2.json"))
            {
                JObject jsonObj = serializer.Deserialize<JObject>(sr.ReadToEnd());
                IEnumerable<JProperty> properties = jsonObj.Properties();
                foreach (JProperty item in properties)
                {
                    Console.WriteLine(item.Name);

                    StationLine2Entity stationLine2Entity = serializer.Deserialize<StationLine2Entity>(item.Value.ToString());
                }
            }

            Console.ReadKey();
        }
    }
}
