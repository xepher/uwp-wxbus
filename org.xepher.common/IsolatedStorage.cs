using System;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using ICSharpCode.SharpZipLib.Zip;
using org.xepher.lang;

namespace org.xepher.common
{
    public static class IsolatedStorage
    {
        // 释放自带数据库
        // 判断原来数据库版本,低于程序版本就更新到新版本
        public static void Zip2IS(Stream zipStream, bool isForced = false)
        {
#if DEBUG
            // 测试自动更新用
            AppSettingHelper.AddOrUpdateValue(StringConstants.VERSION_CODE, Int32Constants.VERSION_CODE_TEST);
#endif

            byte[] data = new byte[2048];
            int size = 2048;

            IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();

            // 判断是否存在wuxitraffic.db
            if (!isolatedStorageFile.FileExists(SQLiteHelper.DATABASE_URI) || isForced)
            {
                using (ZipInputStream zipInputStream = new ZipInputStream(zipStream))
                {
                    ZipEntry zipEntry = zipInputStream.GetNextEntry();

                    if (zipEntry != null)
                    {
                        string fileName = zipEntry.Name;
                        if (fileName != String.Empty)
                        {
                            if (!Directory.Exists(SQLiteHelper.DATABASE_FOLDER_URI)) isolatedStorageFile.CreateDirectory(SQLiteHelper.DATABASE_FOLDER_URI);
                            using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(SQLiteHelper.DATABASE_URI, FileMode.Create, isolatedStorageFile))
                            {
                                Debug.WriteLine("------DebugLog------Release Zip to IsolatedStorageFile Begin------");
                                while (true)
                                {
                                    size = zipInputStream.Read(data, 0, data.Length);
                                    if (size <= 0)
                                    {
                                        Debug.WriteLine("------DebugLog------Release Zip to IsolatedStorageFile End------");
                                        break;
                                    }
                                    fileStream.Write(data, 0, size);
                                }
                                AppSettingHelper.AddOrUpdateValue(StringConstants.VERSION_CODE, Int32Constants.VERSION_CODE);
                            }
                        }
                    }
                }
            }

            isolatedStorageFile.Dispose();
        }

        // 释放更新的android APK
        public static void AppUpdateApk(Stream zipStream)
        {
            byte[] data = new byte[2048];
            int size = 2048;

            IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();

            using (ZipInputStream zipInputStream = new ZipInputStream(zipStream))
            {
                ZipEntry zipEntry;
                do
                {
                    zipEntry = zipInputStream.GetNextEntry();
                    if (zipEntry == null) throw new Exception();
                } while (!zipEntry.Name.ToLower().Contains(".zip"));



                // Unzip zip file
                using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(SQLiteHelper.DATABASE_ZIP_URI, FileMode.Create, isolatedStorageFile))
                {
                    while (true)
                    {
                        size = zipInputStream.Read(data, 0, data.Length);
                        if (size <= 0)
                        {
                            break;
                        }
                        fileStream.Write(data, 0, size);
                    }
                }
            }

            isolatedStorageFile.Dispose();
        }

        // 释放APK中zip包到IsolatedStorage
        public static void AppUpdateDB()
        {
            byte[] data = new byte[2048];
            int size = 2048;

            IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();

            // 备份原始数据库
            isolatedStorageFile.CopyFile(SQLiteHelper.DATABASE_URI, SQLiteHelper.DATABASE_BAK_URI, true);

            using (
                IsolatedStorageFileStream fileStreamZip = new IsolatedStorageFileStream(SQLiteHelper.DATABASE_ZIP_URI,
                                                                                     FileMode.Open, isolatedStorageFile)
                )
            {
                using (ZipInputStream zipInputStream = new ZipInputStream(fileStreamZip))
                {
                    ZipEntry zipEntry = zipInputStream.GetNextEntry();

                    if (zipEntry != null)
                    {
                        string fileName = zipEntry.Name;
                        if (fileName != String.Empty)
                        {
                            using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(SQLiteHelper.DATABASE_URI, FileMode.Create, isolatedStorageFile))
                            {
                                while (true)
                                {
                                    size = zipInputStream.Read(data, 0, data.Length);
                                    if (size <= 0)
                                    {
                                        break;
                                    }
                                    fileStream.Write(data, 0, size);
                                }
                            }
                        }
                    }
                }
            }

            isolatedStorageFile.Dispose();
        }

        // 回滚数据库
        public static void RestoreDB()
        {
            IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();

            // 还原数据库
            isolatedStorageFile.CopyFile(SQLiteHelper.DATABASE_BAK_URI, SQLiteHelper.DATABASE_URI, true);
        }
    }
}
