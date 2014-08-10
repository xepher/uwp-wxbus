/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:MvvmLightSample.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using Framework.NavigationService;
using Framework.Serializer;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Host.Model;
using Host.Utils;
using Microsoft.Practices.ServiceLocation;

namespace Host.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        private static ShellViewModel _shellViewModel;
        private static SegmentViewModel _segmentViewModel;
        private static SettingsViewModel _settingsViewModel;
        private static INavigationService _navigationService;

        static ViewModelLocator()
        {
            IsolatedStorageHelper.InitAllSettings();

            SQLiteHelper.ReleaseDatabaseFile();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ISerializer, JsonConvertSerializer>();
            //Ioc.Container.Register<ISerializer, JsonConvertSerializer>();
            _navigationService =
                new NavigationService(
                    System.Windows.Application.Current.RootVisual as Microsoft.Phone.Controls.PhoneApplicationFrame);
            SimpleIoc.Default.Register<INavigationService>(() => _navigationService);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
                //Ioc.Container.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
                //Ioc.Container.Register<IDataService, DataService>();
            }

            _shellViewModel = new ShellViewModel(_navigationService);
            _segmentViewModel = new SegmentViewModel(_navigationService);
            _settingsViewModel = new SettingsViewModel(_navigationService);

            SimpleIoc.Default.Register<ShellViewModel>(() => _shellViewModel);
            SimpleIoc.Default.Register<SegmentViewModel>(() => _segmentViewModel);
            SimpleIoc.Default.Register<SettingsViewModel>(() => _settingsViewModel);
            //Ioc.Container.RegisterInstance<ShellViewModel>(_shellViewModel);
            //Ioc.Container.RegisterInstance<SegmentViewModel>(_segmentViewModel);
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ShellViewModel Shell
        {
            get
            {
                //return Ioc.Container.Resolve<ShellViewModel>();
                return ServiceLocator.Current.GetInstance<ShellViewModel>();
            }
        }

        /// <summary>
        /// Gets the Segemnts property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SegmentViewModel Segments
        {
            get
            {
                //return Ioc.Container.Resolve<SegmentViewModel>();
                return ServiceLocator.Current.GetInstance<SegmentViewModel>();
            }
        }

        /// <summary>
        /// Gets the Settings property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsViewModel Settings
        {
            get
            {
                //return Ioc.Container.Resolve<SettingsViewModel>();
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}