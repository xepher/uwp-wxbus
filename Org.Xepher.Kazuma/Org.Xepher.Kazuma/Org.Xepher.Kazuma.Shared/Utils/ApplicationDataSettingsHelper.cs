using ReactiveUI;
using Splat;
using System;
using Windows.Storage;

namespace Org.Xepher.Kazuma.Utils
{
    static class ApplicationDataSettingsHelper
    {
        static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        static IMessageBus hostMessageBus = Locator.Current.GetService<IMessageBus>();
        
        public static void SaveOrUpdateValue<T>(string key, T value)
        {
            localSettings.Values[key] = value;

            hostMessageBus.SendMessage<T>(value, key);
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
