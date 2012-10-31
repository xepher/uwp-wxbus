using System.Windows.Controls;
using Microsoft.Phone.Controls;
using org.xepher.lang;

namespace org.xepher.wuxibus
{
    public partial class SettingPage : PhoneApplicationPage
    {
        private bool _isListPickerSelected = false;

        public SettingPage()
        {
            InitializeComponent();

            InitializeControls();

            string lang = AppSettingHelper.GetValueOrDefault("language", "zh-CN");
            for (int index = 0; index < lstPickerLang.Items.Count; index++)
            {
                if (lstPickerLang.Items[index].ToString().ToLower() == lang.ToLower())
                {
                    lstPickerLang.SelectedIndex = index;
                    break;
                }
            }

            lstPickerNetwork.SelectedIndex = AppSettingHelper.GetValueOrDefault("network", 0);

            _isListPickerSelected = true;
        }

        private void InitializeControls()
        {
            lstPickerNetwork.Items.Add(AppResource.SettingsNetworkTelecom);
            lstPickerNetwork.Items.Add(AppResource.SettingsNetworkMobile);
            lstPickerNetwork.Items.Add(AppResource.SettingsNetworkUnicom);
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

        private void lstPickerNetwork_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isListPickerSelected)
            {
                if (lstPickerNetwork.SelectedItem.ToString() == AppResource.SettingsNetworkTelecom)
                {
                    AppSettingHelper.AddOrUpdateValue("network", 0);
                }
                if (lstPickerNetwork.SelectedItem.ToString() == AppResource.SettingsNetworkMobile)
                {
                    AppSettingHelper.AddOrUpdateValue("network", 1);
                }
                if (lstPickerNetwork.SelectedItem.ToString() == AppResource.SettingsNetworkUnicom)
                {
                    AppSettingHelper.AddOrUpdateValue("network", 2);
                }
            }
        }
    }
}