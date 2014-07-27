using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage;
using Host.Model;
using SQLite;

namespace Host.Utils
{
    public class SQLiteHelper
    {
        private static string dbFile = ApplicationData.Current.LocalFolder.Path + "\\database.db";
        //private static string dbFile = "database.db";

        private static SQLiteAsyncConnection GetConn()
        {
            return new SQLiteAsyncConnection(dbFile);
        }

        public static async void InitAllTable()
        {
            if (!IsolatedStorageFile.GetUserStoreForApplication().FileExists(dbFile))
            {
                IsolatedStorageHelper.CopyFromContentToStorage("/Host;component/Assets/database.db", dbFile);

                SQLiteAsyncConnection conn = GetConn();
                await conn.CreateTableAsync<NewsEntity>();
            }
        }

        public async static void SaveNews(IList<NewsEntity> data)
        {
            SQLiteAsyncConnection conn = GetConn();
            if (await conn.Table<NewsEntity>().CountAsync() == 0)
            {
                await conn.InsertAllAsync(data);
            }
            else
            {
                int recordMaxIndex =
                    conn.Table<NewsEntity>().OrderByDescending(entity => entity.Id).FirstAsync().Result.Id;
                foreach (NewsEntity newsEntity in data)
                {
                    if (newsEntity.Id > recordMaxIndex)
                    {
                        await conn.InsertAsync(newsEntity);
                    }
                }
            }
        }

        public async static Task<IList<NewsEntity>> LoadNews()
        {
            SQLiteAsyncConnection conn = GetConn();
            var query = conn.Table<NewsEntity>().OrderByDescending(s=>s.Id);
            var result = query.ToListAsync();

            return result.Result;
        }
    }
}
