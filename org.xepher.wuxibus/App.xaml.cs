using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using org.xepher.common;
using org.xepher.lang;
using org.xepher.model;
using org.xepher.wuxibus.Theme;
using org.xepher.wuxibus.misc;

namespace org.xepher.wuxibus
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        public Line SelectedLine { get; set; }
        public Station SelectedStation { get; set; }
        public Segment SelectedSegment { get; set; }
        public List<Line> Lines { get; set; }

        private SQLiteHelper _dAHelperInstance;
        public SQLiteHelper DAHelperInstance
        {
            get { return _dAHelperInstance ?? (_dAHelperInstance = new SQLiteHelper()); }
        }
        private AdHelper _adHelperInstance;
        internal AdHelper AdHelperInstance
        {
            get { return _adHelperInstance ?? (_adHelperInstance = new AdHelper()); }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Localization initialization
            InitializeLanguage();

            // Theme initialization
            InitializeTheme();

            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        private void InitializeTheme()
        {
            Themes _theme = AppSettingHelper.GetValueOrDefault(StringConstants.THEME, Themes.DarkBlue);
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            if (_theme == Themes.DarkBlue)
            {
                Application.LoadComponent(resourceDictionary,
                                          new Uri("/org.xepher.wuxibus;component/Theme/dark/CustomTheme.xaml",
                                                  UriKind.Relative));
            }
            else
            {
                Application.LoadComponent(resourceDictionary,
                                          new Uri("/org.xepher.wuxibus;component/Theme/light/CustomTheme.xaml",
                                                  UriKind.Relative));
            }
            if (Application.Current.Resources.MergedDictionaries.Count == 0)
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            else
                Application.Current.Resources.MergedDictionaries[0] = resourceDictionary;
        }

        private void InitializeLanguage()
        {
            string _lang = AppSettingHelper.GetValueOrDefault(StringConstants.LANGUAGE, string.Empty);
            if (string.IsNullOrEmpty(_lang))
            {
                AppSettingHelper.AddOrUpdateValue(StringConstants.LANGUAGE, StringConstants.LANGUAGE_ZHCN);
            }
            AppResource.Culture = new CultureInfo(_lang);
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            DAHelperInstance.OpenConnection();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            DAHelperInstance.DisposeConnection();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            DAHelperInstance.DisposeConnection();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            DAHelperInstance.DisposeConnection();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            DAHelperInstance.DisposeConnection();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;

            // 初始化自定义全局Loading
            GlobalLoading.Instance.Initialize(RootFrame);
            GlobalLoading.Instance.LoadingText = AppResource.SystemTrayLoadingText;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}