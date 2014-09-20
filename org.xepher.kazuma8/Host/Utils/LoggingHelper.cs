using System;
using Host.Model;

namespace Host.Utils
{
    public class LoggingHelper
    {
        public static void LogExceptionError(string source, Exception exception)
        {
            BuildExceptionMessage(source, exception);
        }

        private static ExceptionMessage BuildExceptionMessage(string source, Exception exception)
        {
            return new ExceptionMessage()
            {
                Source = source,
                Info = SystemInfoHelper.GetSystemInfo(),
                ExceptionObject = exception
            };
        }
    }
}
