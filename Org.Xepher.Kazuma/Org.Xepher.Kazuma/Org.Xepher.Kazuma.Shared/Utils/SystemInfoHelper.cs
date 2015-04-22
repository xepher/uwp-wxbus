using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration.Pnp;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using System.Linq;

namespace Org.Xepher.Kazuma.Utils
{
    class SystemInfoHelper
    {
        // Device Info
        EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();
        // Hardware Info
        HardwareToken hardwareInfo = HardwareIdentification.GetPackageSpecificToken(null);
        // Package Info
        PackageId appInfo = Package.Current.Id;

        public static class SystemInfo
        {
            public static int ProcessorCount
            {
                get
                {
                    return Environment.ProcessorCount;
                }
            }

            public static int TickCount
            {
                get
                {
                    return Environment.TickCount;
                }
            }

            public static string CultureName
            {
                get
                {
                    return CultureInfo.CurrentCulture.DisplayName;
                }
            }

            public static string Date
            {
                get
                {
                    return DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") + " 星期" + "日一二三四五六七".Substring((int)DateTime.Now.DayOfWeek, 1);
                }
            }

            public static string TimeZone
            {
                get
                {
                    return "UTC" + DateTimeOffset.Now.ToString("%K");
                }
            }

            #region 获取当前系统版本号，摘自：http://attackpattern.com/2013/03/device-information-in-windows-8-store-apps/

            public static async Task<string> GetWindowsVersionAsync()
            {
                var hal = await GetHalDevice(DeviceDriverVersionKey);
                if (hal == null || !hal.Properties.ContainsKey(DeviceDriverVersionKey))
                    return null;

                var versionParts = hal.Properties[DeviceDriverVersionKey].ToString().Split('.');
                return string.Join(".", versionParts.Take(2).ToArray());
            }

            private static async Task<PnpObject> GetHalDevice(params string[] properties)
            {
                var actualProperties = properties.Concat(new[] { DeviceClassKey });
                var rootDevices = await PnpObject.FindAllAsync(PnpObjectType.Device,
                    actualProperties, RootQuery);

                foreach (var rootDevice in rootDevices.Where(d => d.Properties != null && d.Properties.Any()))
                {
                    var lastProperty = rootDevice.Properties.Last();
                    if (lastProperty.Value != null)
                        if (lastProperty.Value.ToString().Equals(HalDeviceClass))
                            return rootDevice;
                }
                return null;
            }

            const string DeviceClassKey = "{A45C254E-DF1C-4EFD-8020-67D146A850E0},10";
            const string DeviceDriverVersionKey = "{A8B865DD-2E3D-4094-AD97-E593A70C75D6},3";
            const string RootContainer = "{00000000-0000-0000-FFFF-FFFFFFFFFFFF}";
            const string RootQuery = "System.Devices.ContainerId:=\"" + RootContainer + "\"";
            const string HalDeviceClass = "4d36e966-e325-11ce-bfc1-08002be10318";

            #endregion
        }
    }
}
