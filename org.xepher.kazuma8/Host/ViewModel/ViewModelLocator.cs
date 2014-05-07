/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:MvvmLightSample.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using Framework.Container;
using Framework.Serializer;
using GalaSoft.MvvmLight;
using Host.Model;
//using wuxibus.ViewModel;
//using wuxibus.ViewModel.DesignViewModel;
using Host.View;

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

        static ViewModelLocator()
        {
            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            Ioc.Container.Register<ISerializer, JsonConvertSerializer>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                //SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
                Ioc.Container.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                //SimpleIoc.Default.Register<IDataService, DataService>();
                Ioc.Container.Register<IDataService, DataService>();
            }

            _shellViewModel = new ShellViewModel();
            _segmentViewModel = new SegmentViewModel();

            Ioc.Container.RegisterInstance(_shellViewModel);
            Ioc.Container.RegisterInstance(_segmentViewModel);
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
                return Ioc.Container.Resolve<ShellViewModel>();
            }
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SegmentViewModel Segments
        {
            get
            {
                return Ioc.Container.Resolve<SegmentViewModel>();
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