using System;
using Windows.Storage;

namespace Org.Xepher.Kazuma.Utils
{
    static class ApplicationDataSettingsHelper
    {
        static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        
        public static void SaveOrUpdateValue<T>(string key, T value)
        {
            localSettings.Values[key] = value;
        }

        public static T ReadValue<T>(string key)
        {
            if (localSettings.Values.ContainsKey(key))
            {
                return (T)localSettings.Values[key];
            }
            else
            {
                return Activator.CreateInstance<T>();
            }
        }
    }
}
