using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.Utils
{
    public class JsonConvertSerializer : ISerializer
    {
        public object Deserialize(Type type, string data)
        {
            return JsonConvert.DeserializeObject(data);
        }

        public T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public string Serialize(object instance)
        {
            return JsonConvert.SerializeObject(instance);
        }
    }
}
