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

        private static async Task<bool> JudgeSQLiteTableExist(string tableName)
        {
            SQLiteAsyncConnection conn = GetConn();
            return conn.ExecuteScalarAsync<bool>(
                string.Format("SELECT COUNT(*) FROM sqlite_master where type='table' and name='{0}'", tableName)).Result;
        }

        public static void RleaseDatabaseFile()
        {
            if (!IsolatedStorageFile.GetUserStoreForApplication().FileExists(dbFile))
            {
                IsolatedStorageHelper.CopyFromContentToStorage("/Host;component/Assets/database.db", dbFile);
            }
        }

        public static async void SaveNews(IList<NewsEntity> data)
        {
            SQLiteAsyncConnection conn = GetConn();
            bool isTableCreated = JudgeSQLiteTableExist("NewsEntity").Result;
            if (!isTableCreated)
            {
                await conn.CreateTableAsync<NewsEntity>();
            }
            else
            {
                await conn.DeleteAsync(conn.Table<NewsEntity>().ToListAsync().Result);
            }
            await conn.InsertAllAsync(data);
        }

        public static async Task<IList<NewsEntity>> LoadNews()
        {
            SQLiteAsyncConnection conn = GetConn();

            var query = conn.Table<NewsEntity>().OrderByDescending(s => s.Id).ToListAsync();

            return query.Result;
        }

        public static async void SaveAllLines(IList<LineEntity> data)
        {
            SQLiteAsyncConnection conn = GetConn();
            bool isTableCreated = JudgeSQLiteTableExist("LineEntity").Result;
            if (!isTableCreated)
            {
                await conn.CreateTableAsync<LineEntity>();
            }
            else
            {
                await conn.DeleteAsync(conn.Table<LineEntity>().ToListAsync().Result);
            }
            await conn.InsertAllAsync(data);
        }

        public static async Task<IList<LineEntity>> LoadAllLines()
        {
            SQLiteAsyncConnection conn = GetConn();

            var query = conn.Table<LineEntity>().ToListAsync();

            return query.Result;
        }
    }
}
