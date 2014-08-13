using System;
using System.Text;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace Host.Utils
{
    public class LoggingHelper
    {
        public static void ShowExceptionError(string title, Exception exception)
        {
            if (MessageBoxResult.OK == MessageBox.Show(Constants.EXCEPTION_HANDLING_MESSAGE_CONTENT, title, MessageBoxButton.OKCancel))
            {
                string msg = BuildExceptionMessage(exception);

                EmailComposeTask task = new EmailComposeTask();
                task.To = "xepher@outlook.com";
                task.Subject = title;
                task.Body = msg;
                task.Show();

                MessageBox.Show(msg, title, MessageBoxButton.OK);
            }
        }

        private static string BuildExceptionMessage(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("以下是程序出错异常信息，发送异常信息给开发者能够帮助开发者迅速定位错误，快速解决错误。");
            sb.Append(Environment.NewLine);
            sb.Append("========= Exception Information Start =========");
            sb.Append(Environment.NewLine);
            sb.Append(string.Format("Message: {0}", exception.Message));
            sb.Append(Environment.NewLine);
            sb.Append("StackTrace:");
            sb.Append(Environment.NewLine);
            sb.Append(exception.StackTrace);
            sb.Append(Environment.NewLine);
            sb.Append("=========  Exception Information End  =========");

            return sb.ToString();
        }
    }
}
