/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:MvvmLightSample.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Host.Model;
using Framework.Navigator;
using Framework.Container;
using Framework.Serializer;
//using wuxibus.ViewModel;
//using wuxibus.ViewModel.DesignViewModel;

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
        static ViewModelLocator()
        {
            //ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //SimpleIoc.Default.Register<INavigator, Navigator>();
            //Ioc.Container.Register<INavigator, Navigator>();
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

            //SimpleIoc.Default.Register<ShellViewModel>();
            Ioc.Container.Register<ShellViewModel>();
            //SimpleIoc.Default.Register<SegmentViewModel>();
            Ioc.Container.Register<SegmentViewModel>();
            //SimpleIoc.Default.Register<StationLine2ViewModel>();
            //SimpleIoc.Default.Register<LineDetailViewModel>();
            //SimpleIoc.Default.Register<NewsViewModel>();
            //SimpleIoc.Default.Register<LineListViewModel>();
            //SimpleIoc.Default.Register<DesignViewModel>();
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
                //return ServiceLocator.Current.GetInstance<ShellViewModel>();
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
                //return ServiceLocator.Current.GetInstance<SegmentViewModel>();
                return Ioc.Container.Resolve<SegmentViewModel>();
            }
        }

        ///// <summary>
        ///// Gets the LineList property.
        ///// </summary>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
        //    "CA1822:MarkMembersAsStatic",
        //    Justification = "This non-static member is needed for data binding purposes.")]
        //public LineListViewModel LineList
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<LineListViewModel>();
        //    }
        //}

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}