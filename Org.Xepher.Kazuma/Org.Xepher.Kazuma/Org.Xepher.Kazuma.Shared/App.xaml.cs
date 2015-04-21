using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ReactiveUI;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Org.Xepher.Kazuma.ViewModels;
using Org.Xepher.Kazuma.Views;
using Splat;
using Org.Xepher.Kazuma.Utils;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace Org.Xepher.Kazuma
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        readonly AutoSuspendHelper autoSuspendHelper;
#if WINDOWS_PHONE_APP
        private TransitionCollection transitions;
#endif

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            autoSuspendHelper = new AutoSuspendHelper(this);

            // Register Appbootstrapper itself, other register should put in Appbootstrapper
            AppBootstrapper bootstrapper = new AppBootstrapper();
            Locator.CurrentMutable.RegisterConstant(bootstrapper, typeof(AppBootstrapper));

            // you should handle the AppState by yourself
            RxApp.SuspensionHost.CreateNewAppState = () => bootstrapper;
            RxApp.SuspensionHost.SetupDefaultSuspendResume();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            base.OnLaunched(e);
            autoSuspendHelper.OnLaunched(e);

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
#if WINDOWS_PHONE_APP
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;
#endif

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (rootFrame.Navigate(typeof(Shell), e.Arguments))
                {
                    IScreen hostScreen = Locator.Current.GetService<IScreen>();
                    IMessageBus hostMessageBus = Locator.Current.GetService<IMessageBus>();
                    // Navigate to the opening page of the application
                    hostScreen.Router.Navigate.Execute(new MainViewModel(hostScreen, hostMessageBus));
                }
                else
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            if (!string.IsNullOrEmpty(e.Arguments))
            {
                // if only get service, use Locator.Current is enough
                IScreen hostScreen = Locator.Current.GetService<IScreen>();
                IMessageBus hostMessageBus = Locator.Current.GetService<IMessageBus>();
                if (hostScreen.Router.NavigationStack.Count > 1)
                {
                    hostScreen.Router.NavigationStack.RemoveRange(1, hostScreen.Router.NavigationStack.Count - 1);
                }
                hostScreen.Router.Navigate.Execute(new RouteViewModel(hostScreen, hostMessageBus, SecondaryTileHelper.PrepareNavigationParameter(e.TileId, e.Arguments)));
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

#if WINDOWS_PHONE_APP
        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }
#endif

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}