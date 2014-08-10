using System;
using System.Diagnostics;
using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;

namespace Host.Utils
{
    public class SystemInfoHelper
    {
        private const int ANID_LENGTH = 32;
        private const int ANID_OFFSET = 2;

        public static void GetSystemInfo()
        {
            Debug.WriteLine("====== Start Get System Info ======");
            Debug.WriteLine("DeviceManufacturer: {0}", DeviceStatus.DeviceManufacturer);
            Debug.WriteLine("DeviceName: {0}", DeviceStatus.DeviceName);
            Debug.WriteLine("DeviceFirmwareVersion: {0}", DeviceStatus.DeviceFirmwareVersion);
            Debug.WriteLine("DeviceHardwareVersion: {0}", DeviceStatus.DeviceHardwareVersion);
            Debug.WriteLine("DeviceTotalMemory: {0}", DeviceStatus.DeviceTotalMemory);
            Debug.WriteLine("ApplicationCurrentMemoryUsage: {0}", DeviceStatus.ApplicationCurrentMemoryUsage);
            Debug.WriteLine("ApplicationPeakMemoryUsage: {0}", DeviceStatus.ApplicationPeakMemoryUsage);
            Debug.WriteLine("ApplicationMemoryUsageLimit: {0}", DeviceStatus.ApplicationMemoryUsageLimit);
            Debug.WriteLine("IsKeyboardPresent: {0}", DeviceStatus.IsKeyboardPresent);
            Debug.WriteLine("IsKeyboardDeployed: {0}", DeviceStatus.IsKeyboardDeployed);
            Debug.WriteLine("PowerSource: {0}", DeviceStatus.PowerSource);
            // ID_CAP_IDENTITY_DEVICE
            Debug.WriteLine("DeviceName: {0}", GetDeviceUniqueID());
            Constants.DEVICE_TOKEN = SignatureUtil.GetMD5HashString(GetDeviceUniqueID());
            // ID_CAP_IDENTITY_USER
            Debug.WriteLine("ANID: {0}", GetWindowsLiveAnonymousID());
            Debug.WriteLine("Device Width: {0}", System.Windows.Application.Current.Host.Content.ActualWidth);
            Debug.WriteLine("Device Height: {0}", System.Windows.Application.Current.Host.Content.ActualHeight);
            Debug.WriteLine("OS Version: {0}", Environment.OSVersion);
            Debug.WriteLine("CellularMobileOperator: {0}", DeviceNetworkInformation.CellularMobileOperator);
            Debug.WriteLine("IsCellularDataEnabled: {0}", DeviceNetworkInformation.IsCellularDataEnabled);
            Debug.WriteLine("IsCellularDataRoamingEnabled: {0}", DeviceNetworkInformation.IsCellularDataRoamingEnabled);
            Debug.WriteLine("IsNetworkAvailable: {0}", DeviceNetworkInformation.IsNetworkAvailable);
            Debug.WriteLine("IsWiFiEnabled: {0}", DeviceNetworkInformation.IsWiFiEnabled);
            Debug.WriteLine("====== End Get System Info ======");
        }

        private static string GetDeviceUniqueID()
        {
            object uniqueId;

            if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uniqueId) && null != uniqueId)
            {
                return BitConverter.ToString((byte[])uniqueId);
            }
            return "null";
        }

        private static string GetWindowsLiveAnonymousID()
        {
            object anid;

            if (UserExtendedProperties.TryGetValue("ANID", out anid) && null != anid && anid.ToString().Length >= (ANID_LENGTH + ANID_OFFSET))
            {
                return anid.ToString().Substring(ANID_OFFSET, ANID_LENGTH);
            }
            return "null";
        }

    }
}
