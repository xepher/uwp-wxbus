using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Settings
{
    public sealed class Settings : ISettings
    {
        public void AddOrUpdate(string key, object value)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = value;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return IsolatedStorageSettings.ApplicationSettings.TryGetValue<T>(key, out value);
        }


        public bool Remove(string key)
        {
            var result = IsolatedStorageSettings.ApplicationSettings.Remove(key);
            IsolatedStorageSettings.ApplicationSettings.Save();
            return result;
        }

        public bool ContainsKey(string key)
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(key);
        }
    }
}
