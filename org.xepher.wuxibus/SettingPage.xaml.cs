using System.Globalization;
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
    }
}