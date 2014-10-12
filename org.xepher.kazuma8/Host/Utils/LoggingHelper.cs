using System;
using Host.Model;

namespace Host.Utils
{
    public class LoggingHelper
    {
        public static async void LogExceptionError(string source, bool isDebugMode, Exception exception)
        {
            ExceptionMessage message = BuildExceptionMessage(source, isDebugMode, exception);
            await SQLiteHelper.SaveExceptionMessage(message);
        }

        private static ExceptionMessage BuildExceptionMessage(string source, bool isDebugMode, Exception exception)
        {
            SystemInfo info = SystemInfoHelper.GetSystemInfo();
            return new ExceptionMessage()
            {
                Source = source,
                DeviceManufacturer = info.DeviceManufacturer,
                DeviceName = info.DeviceName,
                DeviceFirmwareVersion = info.DeviceFirmwareVersion,
                DeviceHardwareVersion = info.DeviceHardwareVersion,
                DeviceTotalMemory = info.DeviceTotalMemory,
                ApplicationCurrentMemoryUsage = info.ApplicationCurrentMemoryUsage,
                ApplicationPeakMemoryUsage = info.ApplicationPeakMemoryUsage,
                ApplicationMemoryUsageLimit = info.ApplicationMemoryUsageLimit,
                IsKeyboardPresent = info.IsKeyboardPresent,
                IsKeyboardDeployed = info.IsKeyboardDeployed,
                PowerSource = info.PowerSource,
                DeviceIdentityName = info.DeviceIdentityName,
                DeviceToken = info.DeviceToken,
                DeviceIdentityUser = info.DeviceIdentityUser,
                DeviceWidth = info.DeviceWidth,
                DeviceHeight = info.DeviceHeight,
                OSVersion = info.OSVersion,
                CellularMobileOperator = info.CellularMobileOperator,
                IsCellularDataEnabled = info.IsCellularDataEnabled,
                IsCellularDataRoamingEnabled = info.IsCellularDataRoamingEnabled,
                IsNetworkAvailable = info.IsNetworkAvailable,
                IsWiFiEnabled = info.IsWiFiEnabled,
                Message = exception.Message,
                ExceptionSource = exception.Source,
                StackTrace = exception.StackTrace,
                IsDebugMode = isDebugMode,
                OccurTime = DateTime.Now,
                IsHandled = false
            };
        }
    }
}
