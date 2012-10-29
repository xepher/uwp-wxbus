using System.IO.IsolatedStorage;

namespace org.xepher.lang
{
    public static class AppSettingHelper
    {
        private static readonly IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        public static void AddOrUpdateValue(string key,object value)
        {
            bool _valueChanged = false;

            if(settings.Contains(key))
            {
                if(settings[key]!=value)
                {
                    settings[key] = value;
                    _valueChanged = true;
                }
            }
            else
            {
                settings.Add(key, value);
                _valueChanged = true;
            }
            if(_valueChanged)
            {
                Save();
            }
        }

        public static T GetValueOrDefault<T>(string key,T defaultValue)
        {
            T value;

            if(settings.Contains(key))
            {
                value = (T) settings[key];
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }

        private static void Save()
        {
            settings.Save();
        }
    }
}
