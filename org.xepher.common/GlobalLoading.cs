using System.Windows.Media;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace org.xepher.common
{
    /// <summary>
    /// Source Code from jeff
    /// http://www.jeff.wilcox.name/2011/07/creating-a-global-progressindicator-experience-using-the-windows-phone-7-1-sdk-beta-2/
    /// modified by xepher
    /// </summary>
    public class GlobalLoading : INotifyPropertyChanged
    {
        private ProgressIndicator _mangoIndicator;

        public string LoadingText { get; set; }

        private GlobalLoading()
        {
        }

        public void Initialize(PhoneApplicationFrame frame)
        {
            // If using AgFx:
            // DataManager.Current.PropertyChanged += OnDataManagerPropertyChanged;

            _mangoIndicator = new ProgressIndicator();

            frame.Navigated += OnRootFrameNavigated;
        }

        private void OnRootFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Use in Mango to share a single progress indicator instance.
            var ee = e.Content;
            var pp = ee as PhoneApplicationPage;
            if (pp != null)
            {
                pp.SetValue(SystemTray.ProgressIndicatorProperty, _mangoIndicator);
            }
        }

        private void OnDataManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsLoading" == e.PropertyName)
            {
                // if AgFx: IsDataManagerLoading = DataManager.Current.IsLoading;
                NotifyValueChanged();
            }
        }

        private static GlobalLoading _in;
        public static GlobalLoading Instance
        {
            get
            {
                if (_in == null)
                {
                    _in = new GlobalLoading();
                }

                return _in;
            }
        }

        public bool IsDataManagerLoading { get; set; }

        public bool ActualIsLoading
        {
            get
            {
                return IsLoading || IsDataManagerLoading;
            }
        }

        private int _loadingCount;

        public bool IsLoading
        {
            get
            {
                return _loadingCount > 0;
            }
            set
            {
                bool loading = IsLoading;
                if (value)
                {
                    ++_loadingCount;
                    _mangoIndicator.Text = LoadingText;
                }
                else
                {
                    --_loadingCount;
                    _mangoIndicator.Text = string.Empty;
                }

                NotifyValueChanged();
            }
        }

        private void NotifyValueChanged()
        {
            if (_mangoIndicator != null)
            {
                _mangoIndicator.IsIndeterminate = _loadingCount > 0 || IsDataManagerLoading;

                // for now, just make sure it's always visible.
                if (_mangoIndicator.IsVisible == false)
                {
                    _mangoIndicator.IsVisible = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
