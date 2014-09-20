namespace Host.Model
{
    public class SystemInfo
    {
        public string DeviceManufacturer { get; set; }
        public string DeviceName { get; set; }
        public string DeviceFirmwareVersion { get; set; }
        public string DeviceHardwareVersion { get; set; }
        public string DeviceTotalMemory { get; set; }
        public string ApplicationCurrentMemoryUsage { get; set; }
        public string ApplicationPeakMemoryUsage { get; set; }
        public string ApplicationMemoryUsageLimit { get; set; }
        public string IsKeyboardPresent { get; set; }
        public string IsKeyboardDeployed { get; set; }
        public string PowerSource { get; set; }
        public string DeviceIdentityName { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceIdentityUser { get; set; }
        public string DeviceWidth { get; set; }
        public string DeviceHeight { get; set; }
        public string OSVersion { get; set; }
        public string CellularMobileOperator { get; set; }
        public string IsCellularDataEnabled { get; set; }
        public string IsCellularDataRoamingEnabled { get; set; }
        public string IsNetworkAvailable { get; set; }
        public string IsWiFiEnabled { get; set; }
    }
}
