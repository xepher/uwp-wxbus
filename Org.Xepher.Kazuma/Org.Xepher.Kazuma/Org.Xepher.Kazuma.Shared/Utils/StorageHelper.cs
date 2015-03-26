using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Org.Xepher.Kazuma.Utils
{
    static class StorageHelper
    {
        // Write data to a file
        internal static async void WriteData<T>(StorageFolder targetFolder, string fileName, T data)
        {
            StorageFile dataFile = await targetFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            ISerializer serializer = new JsonConvertSerializer();
            string serializedResult = serializer.Serialize(data);
            await FileIO.WriteTextAsync(dataFile, serializedResult);
        }

        // Read data from a file
        internal static async Task<T> ReadData<T>(StorageFolder targetFolder, string filename)
        {
            try
            {
                StorageFile dataFile = await targetFolder.GetFileAsync(filename);
                string deserializedResult = await FileIO.ReadTextAsync(dataFile);
                ISerializer serializer = new JsonConvertSerializer();
                return serializer.Deserialize<T>(deserializedResult);
            }
            catch (Exception)
            {
                return Activator.CreateInstance<T>();
            }
        }
    }
}
