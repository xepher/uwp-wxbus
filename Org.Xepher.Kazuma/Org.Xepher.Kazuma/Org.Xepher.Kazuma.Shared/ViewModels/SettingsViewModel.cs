using Org.Xepher.Kazuma.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        ApplicationDataContainer localSettings = null;

        public SettingsViewModel(IScreen screen)
            : base(screen)
        {
            base.PathSegment = "Settings";

            this.localSettings = ApplicationData.Current.LocalSettings;

            #region Local Storage Configuration

            // IsLocalStorageOn default value
            _isLocalStorageOn = ApplicationDataSettingsHelper.ReadValue<bool>("IsLocalStorageOn");

            this.WhenAnyValue(vm => vm.IsLocalStorageOn)
                .Subscribe(x =>
                {
                    ApplicationDataSettingsHelper.SaveOrUpdateValue("IsLocalStorageOn", x);
                });

            #endregion Local Storage Configuration
        }

        #region Local Storage

        private bool _isLocalStorageOn;
        public bool IsLocalStorageOn
        {
            get { return _isLocalStorageOn; }
            set { this.RaiseAndSetIfChanged(ref _isLocalStorageOn, value); }
        }

        #endregion Local Storage
    }
}
