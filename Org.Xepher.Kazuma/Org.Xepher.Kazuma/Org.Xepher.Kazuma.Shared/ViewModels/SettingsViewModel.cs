using Org.Xepher.Kazuma.Common;
using Org.Xepher.Kazuma.Utils;
using ReactiveUI;
using System;
using Windows.Storage;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        ApplicationDataContainer localSettings = null;

        public SettingsViewModel(IAppBootstrapper bootstrapper, IMessageBus messageBus)
            : base(bootstrapper, messageBus)
        {
            base.PathSegment = Constants.PATH_SEGMENT_SETTINGS;

            this.localSettings = ApplicationData.Current.LocalSettings;

            #region Local Storage Configuration

            // IsLocalStorageOn default value
            _isLocalStorageOn = ApplicationDataSettingsHelper.ReadValue<bool>(Constants.SETTINGS_IS_LOCALSTORAGE_ENABLED);

            this.ObservableForProperty(vm => vm.IsLocalStorageOn)
                .Subscribe(x =>
                {
                    ApplicationDataSettingsHelper.SaveOrUpdateValue(Constants.SETTINGS_IS_LOCALSTORAGE_ENABLED, x.Value);
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
