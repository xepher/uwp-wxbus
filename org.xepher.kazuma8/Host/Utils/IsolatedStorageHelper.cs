using System;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Resources;

namespace Host.Utils
{
    public static class IsolatedStorageHelper
    {
        private static IsolatedStorageSettings _settings;

        public static IsolatedStorageSettings Settings
        {
            get
            {
                if (null == _settings)
                {
                    _settings = IsolatedStorageSettings.ApplicationSettings;
                }
                return _settings;
            }
        }

        public static void InitAllSettings()
        {
            if (!Settings.Contains("AnnouncementCircle"))
            {
                Settings["AnnouncementCircle"] = new AnnounceUpdateCircle {Circle = "1天", Hours = 24};
            }
            if (!Settings.Contains("LastNewsUpdateTime"))
            {
                Settings["LastNewsUpdateTime"] = DateTime.MinValue;
            }
        }

        public static void AddOrUpdateSettings(string key, object data)
        {
            if (Settings.Contains(key))
            {
                Settings[key] = data;
            }
            else
            {
                Settings.Add(key, data);
            }
            Settings.Save();
        }

        public static void RemoveSettings(string key)
        {
            if (Settings.Contains(key))
            {
                Settings.Remove(key);
                Settings.Save();
            }
        }

        public static void CopyFileTo(string path, string file)
        {
            using (IsolatedStorageFile isolatedStorageStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isolatedStorageStore.FileExists(Path.Combine(path, file)))
                {
                    using (var isoStream = new IsolatedStorageFileStream(Path.Combine(path, file), FileMode.Create, isolatedStorageStore))
                    {
                        using (var writer = new BinaryWriter(isoStream))
                        {
                            StreamResourceInfo sri = Application.GetResourceStream(new Uri("Assets\\database.db", UriKind.Relative));

                            using (BinaryReader reader = new BinaryReader(sri.Stream))
                            {
                                long len = sri.Stream.Length;
                                for (long i = 0; i < len; i++)
                                    writer.Write(reader.ReadByte());
                            }
                        }
                    }
                }
            }
        }

        public static void CopyFromContentToStorage(String sourceFile, String destinationFile)
        {
            IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
            using (Stream inputStream = Application.GetResourceStream(new Uri(sourceFile, UriKind.Relative)).Stream)
            {
                using (IsolatedStorageFileStream outStream = new IsolatedStorageFileStream(destinationFile, FileMode.Create, FileAccess.Write, isolatedStorageFile))
                {
                    Byte[] Buffer = new Byte[5120];
                    Int32 ReadCount = inputStream.Read(Buffer, 0, Buffer.Length);
                    while (ReadCount > 0)
                    {
                        outStream.Write(Buffer, 0, ReadCount);
                        ReadCount = inputStream.Read(Buffer, 0, Buffer.Length);
                    }
                }
            }
        }
    }
}
