using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Controls;
using org.xepher.common;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class SettingPage : PhoneApplicationPage
    {
        private App _app;
        private readonly bool _isListPickerSelected;
        private bool _isUpdateFailured;

        public SettingPage()
        {
            InitializeComponent();

            _app = (Application.Current as App);

            string lang = AppSettingHelper.GetValueOrDefault("language", "zh-CN");
            for (int index = 0; index < lstPickerLang.Items.Count; index++)
            {
                if (lstPickerLang.Items[index].ToString().ToLower() == lang.ToLower())
                {
                    lstPickerLang.SelectedIndex = index;
                    break;
                }
            }

            _isListPickerSelected = true;
        }

        private void lstPickerLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isListPickerSelected)
            {
                switch (lstPickerLang.SelectedItem.ToString())
                {
                    case "en-US":
                        AppSettingHelper.AddOrUpdateValue("language", "en-US");
                        break;
                    case "zh-CN":
                        AppSettingHelper.AddOrUpdateValue("language", "zh-CN");
                        break;
                }
            }
        }

        private void BtnUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            btnUpdate.IsEnabled = false;
            // 释放sqlite连接
            _app.DAHelperInstance.DisposeConnection();
            GlobalLoading.Instance.IsLoading = true;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://221.130.60.79:8080/Bus/update.xml");

            request.BeginGetResponse(ResponseCallback, request);
        }

        // load update.xml
        private void ResponseCallback(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);

                AppUpdate appUpdateInfo;
                using (Stream stream = response.GetResponseStream())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AppUpdate));
                    try
                    {
                        appUpdateInfo = (AppUpdate)serializer.Deserialize(stream);
                    }
                    catch (Exception)
                    {
                        appUpdateInfo = null;
                    }
                }

                if (appUpdateInfo != null)
                {
                    if (appUpdateInfo.VersionCode > AppSettingHelper.GetValueOrDefault("VersionCode", 27))
                    {
                        Dispatcher.BeginInvoke(() =>
                            {
                                // 提示更新信息,让用户选择是否更新
                                MessageBoxResult result =
                                    MessageBox.Show(BuildUpdateInfo(appUpdateInfo), string.Empty, MessageBoxButton.OKCancel);
                                if (MessageBoxResult.OK == result)
                                {
                                    // 更新
                                    HttpWebRequest requestApp = (HttpWebRequest)WebRequest.Create(appUpdateInfo.Url);

                                    // load apk
                                    requestApp.BeginGetResponse(iar =>
                                    {
                                        try
                                        {
                                            HttpWebRequest requestApk = (HttpWebRequest)iar.AsyncState;
                                            HttpWebResponse responseApk = (HttpWebResponse)requestApk.EndGetResponse(iar);

                                            using (Stream stream = responseApk.GetResponseStream())
                                            {
                                                // 释放apk中zip包,会替换掉原来的zip包
                                                IsolatedStorage.AppUpdateApk(stream);
                                            }

                                            // 释放zip包中db,会替换掉原来的db
                                            IsolatedStorage.AppUpdateDB();
                                            AppSettingHelper.AddOrUpdateValue("VersionCode", appUpdateInfo.VersionCode);

                                            Dispatcher.BeginInvoke(() =>
                                                {
                                                    // 更新完毕
                                                    btnUpdate.Content = string.Format(AppResource.SettingsBtnUpdate,
                                                                                      AppSettingHelper.GetValueOrDefault
                                                                                          ("VersionCode", 27));

                                                    ToastPrompt toast = new ToastPrompt();
                                                    toast.Message = AppResource.SettingsUpdateSuccessed;
                                                    toast.Show();
                                                });
                                        }
                                        catch (Exception)
                                        {
                                            // 设置替换文件出错flag
                                            _isUpdateFailured = true;
                                        }
                                        finally
                                        {
                                            if (_isUpdateFailured)
                                            {
                                                // 如果替换文件出错,回滚
                                                IsolatedStorage.RestoreDB();
                                                Dispatcher.BeginInvoke(() =>
                                                    {
                                                        ToastPrompt toast = new ToastPrompt();
                                                        toast.Message = AppResource.SettingsUpdateFailured;
                                                        toast.Show();
                                                    });
                                            }

                                            Dispatcher.BeginInvoke(() =>
                                                {
                                                    GlobalLoading.Instance.IsLoading = false;
                                                    // 打开sqlite连接
                                                    _app.DAHelperInstance.OpenConnection();
                                                    btnUpdate.IsEnabled = true;
                                                });
                                        }
                                    }, requestApp);
                                }
                                else
                                {
                                    Dispatcher.BeginInvoke(() =>
                                        {
                                            // 用户不更新
                                            GlobalLoading.Instance.IsLoading = false;
                                            // 打开sqlite连接
                                            _app.DAHelperInstance.OpenConnection();
                                            btnUpdate.IsEnabled = true;
                                        });
                                }
                            });
                    }
                    else
                    {
                        Dispatcher.BeginInvoke(() =>
                            {
                                ToastPrompt toast = new ToastPrompt();
                                toast.Message = AppResource.SettingsUpdateAlreadyLatest;
                                toast.Show();

                                // 已经是最新版,不用更新
                                GlobalLoading.Instance.IsLoading = false;
                                // 打开sqlite连接
                                _app.DAHelperInstance.OpenConnection();
                                btnUpdate.IsEnabled = true;
                            });
                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(() =>
                        {
                            ToastPrompt toast = new ToastPrompt();
                            toast.Message = AppResource.SettingsUpdateFetchInfoError;
                            toast.Show();

                            // 读取更新信息失败
                            GlobalLoading.Instance.IsLoading = false;
                            // 打开sqlite连接
                            _app.DAHelperInstance.OpenConnection();
                            btnUpdate.IsEnabled = true;
                        });
                }
            }
            catch (Exception)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    ToastPrompt toast = new ToastPrompt();
                    toast.Message = AppResource.SettingsUpdateUnknownError;
                    toast.Show();

                    // 读取更新信息失败
                    GlobalLoading.Instance.IsLoading = false;
                    // 打开sqlite连接
                    _app.DAHelperInstance.OpenConnection();
                    btnUpdate.IsEnabled = true;
                });
            }
        }

        private string BuildUpdateInfo(AppUpdate appUpdateInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(
                appUpdateInfo.Desc.Replace("|", "\n")
                             .Replace(",", "\n")
                             .Replace("【", "\n")
                             .Replace("】", "\n")
                             .Replace(" ", ""));
            sb.Append("\n\n");
            sb.Append(AppResource.SettingsUpdateTip);
            return sb.ToString();
        }

        private void SettingPage_OnBackKeyPress(object sender, CancelEventArgs e)
        {
            if (GlobalLoading.Instance.IsLoading) e.Cancel = true;
        }

        //protected override void OnBackKeyPress(CancelEventArgs e)
        //{
        //    if (_isListPickerSelected)
        //    {
        //        string lang = AppSettingHelper.GetValueOrDefault("language", "zh-CN");
        //        CultureInfo ci = new CultureInfo(lang);
        //        AppResource.Culture = Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = ci;
        //        while (NavigationService.CanGoBack)
        //        {
        //            NavigationService.RemoveBackEntry();
        //        }
        //        NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        //    }
        //    base.OnBackKeyPress(e);
        //}
    }
}