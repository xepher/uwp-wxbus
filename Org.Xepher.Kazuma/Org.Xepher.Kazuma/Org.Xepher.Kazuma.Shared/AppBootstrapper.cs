using Org.Xepher.Kazuma.Common;
using Org.Xepher.Kazuma.Utils;
using Org.Xepher.Kazuma.Utils.Logger;
using Org.Xepher.Kazuma.ViewModels;
using Org.Xepher.Kazuma.Views;
using ReactiveUI;
using Splat;
using System;
using Windows.Devices.Geolocation;

namespace Org.Xepher.Kazuma
{
    /* COOLSTUFF: What is the AppBootstrapper?
     * 
     * The AppBootstrapper is like a ViewModel for the WPF Application class.
     * Since Application isn't very testable (just like Window / UserControl), 
     * we want to create a class we can test. Since our application only has
     * one "screen" (i.e. a place we present Routed Views), we can also use 
     * this as our IScreen.
     * 
     * An IScreen is a ViewModel that contains a Router - practically speaking,
     * it usually represents a Window (or the RootFrame of a WinRT app). We 
     * should technically create a MainWindowViewModel to represent the IScreen,
     * but there isn't much benefit to split those up unless you've got multiple
     * windows.
     * 
     * AppBootstrapper is a good place to implement a lot of the "global 
     * variable" type things in your application. It's also the place where
     * you should configure your IoC container. And finally, it's the place 
     * which decides which View to Navigate to when the application starts.
     */

    public class AppBootstrapper : ReactiveObject, IAppBootstrapper
    {
        public RoutingState Router { get; private set; }

        private BasicGeoposition _myPosition;
        public BasicGeoposition MyPosition
        {
            get { return _myPosition; }
            set { this.RaiseAndSetIfChanged(ref _myPosition, value); }
        }

        public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null)
        {
            Router = testRouter ?? new RoutingState();
            dependencyResolver = dependencyResolver ?? Locator.CurrentMutable;

            // Bind 
            RegisterParts(dependencyResolver);

            // TODO: This is a good place to set up any other app 
            // startup tasks, like setting the logging level
#if DEBUG
            LogHost.Default.Level = LogLevel.Debug;
#else
            LogHost.Default.Level = LogLevel.Info;
#endif
        }

        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
#if DEBUG
            dependencyResolver.RegisterConstant(new ConsoleLogger(), typeof(ILogger));
#endif
            dependencyResolver.RegisterConstant(this, typeof(IAppBootstrapper));
            dependencyResolver.RegisterConstant(new MessageBus(), typeof(IMessageBus));

            dependencyResolver.Register(() => new MainView(), typeof(IViewFor<MainViewModel>));
#if WINDOWS_PHONE_APP
            dependencyResolver.Register(() => new RouteView(), typeof(IViewFor<RouteViewModel>));
#endif
            dependencyResolver.Register(() => new SettingsView(), typeof(IViewFor<SettingsViewModel>));
            dependencyResolver.Register(() => new IAPView(), typeof(IViewFor<IAPViewModel>));

            IMessageBus messageBus = Locator.Current.GetService<IMessageBus>();
            messageBus.Listen<BasicGeoposition>(Constants.MSGBUS_TOKEN_MY_GEOPOSITION).Subscribe(x => MyPosition = x);
        }
    }
}
