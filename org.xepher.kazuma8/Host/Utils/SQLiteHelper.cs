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

        private static SQLiteAsyncConnection GetAsyncConn()
        {
            return new SQLiteAsyncConnection(dbFile);
        }

        private static Task<bool> JudgeSQLiteTableExist(string tableName)
        {
            SQLiteAsyncConnection conn = GetAsyncConn();
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

        public static async Task SaveNews(IEnumerable<NewsEntity> data)
        {
            SQLiteAsyncConnection asyncConn = GetAsyncConn();
            bool isTableCreated = await JudgeSQLiteTableExist("NewsEntity");
            await asyncConn.RunInTransactionAsync(conn =>
            {
                if (!isTableCreated)
                {
                    conn.CreateTable<NewsEntity>();
                }
                else
                {
                    conn.DeleteAll<NewsEntity>();
                }
                // can not use InsertAll, bucause in InsertAll, framework will create a new transaction
                foreach (NewsEntity item in data)
                {
                    conn.Insert(item);
                }
            });
        }

        public static async Task<IList<NewsEntity>> LoadNews()
        {
            SQLiteAsyncConnection asyncConn = GetAsyncConn();

            return await asyncConn.Table<NewsEntity>().OrderByDescending(s => s.Id).ToListAsync();
        }

        public static async Task SaveLines(IEnumerable<LineEntity> data)
        {
            SQLiteAsyncConnection asyncConn = GetAsyncConn();
            bool isTableCreated = await JudgeSQLiteTableExist("LineEntity");
            await asyncConn.RunInTransactionAsync(conn =>
            {
                if (!isTableCreated)
                {
                    conn.CreateTable<LineEntity>();
                }
                else
                {
                    conn.DeleteAll<LineEntity>();
                }
                // can not use InsertAll, bucause in InsertAll, framework will create a new transaction
                foreach (LineEntity item in data)
                {
                    conn.Insert(item);
                }
            });
        }

        public static async Task<IList<LineEntity>> LoadLines()
        {
            SQLiteAsyncConnection asyncConn = GetAsyncConn();

            return await asyncConn.Table<LineEntity>().ToListAsync();
        }
    }
}
