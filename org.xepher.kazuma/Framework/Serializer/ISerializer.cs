using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Serializer
{
    /// <summary>
    /// use Json.net, and you can find documents at
    /// http://james.newtonking.com/json/help/index.html
    /// </summary>
    public interface ISerializer
    {
        object Deserialize(Type type, string data);

        T Deserialize<T>(string data);

        string Serialize(object instance);
    }
}
