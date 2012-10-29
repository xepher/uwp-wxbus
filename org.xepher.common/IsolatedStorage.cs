using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Xml.Serialization;

namespace org.xepher.common
{
    public class IsolatedStorage
    {
        private static void Serialize(Stream streamObject, object objForSerialization)
        {
            if (objForSerialization == null || streamObject == null)
                return;

            XmlSerializer serializer = new XmlSerializer(objForSerialization.GetType());
            serializer.Serialize(streamObject, objForSerialization);
        }

        private static object Deserialize(Stream streamObject, Type serializedObjectType)
        {
            if (serializedObjectType == null || streamObject == null)
                return null;

            XmlSerializer serializer = new XmlSerializer(serializedObjectType);
            return serializer.Deserialize(streamObject);
        }

        // 保存到独立存储，同时检查独立存储中的公交线路数目是否更新，如有更新需要写入
        public static void SaveToFile(object obj, string path)
        {
            IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
            string[] pathArray = path.Split('\\');
            StringBuilder sbDirectory = new StringBuilder();
            for (int index = 0; index < pathArray.Length - 1; index++)
            {
                sbDirectory.Append(pathArray[index]);
            }
            if (!isolatedStorageFile.DirectoryExists(sbDirectory.ToString()))
                isolatedStorageFile.CreateDirectory(sbDirectory.ToString());
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(path, FileMode.Create, isolatedStorageFile))
            {
                Serialize(stream, obj);
            }
        }

        public static Object ReadFromFile(string path,Type type)
        {
            IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
            if (!isolatedStorageFile.FileExists(path)) return null;
            object deserializeObj;
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(path, FileMode.Open, isolatedStorageFile))
            {
                deserializeObj = Deserialize(stream, type);
            }

            return deserializeObj;
        }
    }
}
