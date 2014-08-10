using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Windows.Storage;
using Host.Model;
using SQLite;

namespace Host.Utils
{
    public class SQLiteHelper
    {
        private static string dbFile = ApplicationData.Current.LocalFolder.Path + "\\database.db";

        private static SQLiteAsyncConnection GetConn()
        {
            return new SQLiteAsyncConnection(dbFile);
        }

        private static Task<bool> JudgeSQLiteTableExist(string tableName)
        {
            SQLiteAsyncConnection conn = GetConn();
            return conn.ExecuteScalarAsync<bool>(
                string.Format("SELECT COUNT(*) FROM sqlite_master where type='table' and name='{0}'", tableName));
        }

        public static void ReleaseDatabaseFile()
        {
            if (!IsolatedStorageFile.GetUserStoreForApplication().FileExists(dbFile))
            {
                IsolatedStorageHelper.CopyFromContentToStorage("/Host;component/Assets/database.db", dbFile);
            }
        }

        public static async void SaveNews(IList<NewsEntity> data)
        {
            SQLiteAsyncConnection conn = GetConn();
            bool isTableCreated = await JudgeSQLiteTableExist("NewsEntity");
            if (!isTableCreated)
            {
                await conn.CreateTableAsync<NewsEntity>();
            }
            else
            {
                foreach (var item in await conn.Table<NewsEntity>().ToListAsync())
                {
                    await conn.DeleteAsync(item);
                }
            }
            await conn.InsertAllAsync(data);
        }

        public static async Task<IList<NewsEntity>> LoadNews()
        {
            SQLiteAsyncConnection conn = GetConn();

            return await conn.Table<NewsEntity>().OrderByDescending(s => s.Id).ToListAsync();
        }

        public static async void SaveLines(IList<LineEntity> data)
        {
            SQLiteAsyncConnection conn = GetConn();
            bool isTableCreated = await JudgeSQLiteTableExist("LineEntity");
            if (!isTableCreated)
            {
                await conn.CreateTableAsync<LineEntity>();
            }
            else
            {
                foreach (var item in await conn.Table<LineEntity>().ToListAsync())
                {
                    await conn.DeleteAsync(item);
                }
            }
            await conn.InsertAllAsync(data);
        }

        public static async Task<IList<LineEntity>> LoadLines()
        {
            SQLiteAsyncConnection conn = GetConn();

            return await conn.Table<LineEntity>().ToListAsync();
        }
    }
}
