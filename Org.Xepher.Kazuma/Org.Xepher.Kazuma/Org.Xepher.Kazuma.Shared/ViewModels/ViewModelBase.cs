using Org.Xepher.Kazuma.Common;
using Org.Xepher.Kazuma.Utils;
using ReactiveUI;
using Splat;

namespace Org.Xepher.Kazuma.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel, IEnableLogger
    {
        public string PathSegment { get; protected set; }

        /* COOLSTUFF: What is UrlPathSegment
         * 
         * Imagine that the router state is like the path of the URL - what 
         * would the path look like for this particular page? Maybe it would be
         * the current user's name, or an "id". In this case, it's just a 
         * constant. You can get the whole path via 
         * IRoutingState.GetUrlForCurrentRoute.
         */
        public string UrlPathSegment
        {
            get { return PathSegment; }
        }

        /// <summary>
        /// Don't use this IScreen object, use HostBootstrapper instead
        /// </summary>
        public IScreen HostScreen
        {
            get;
            protected set;
        }

        public IAppBootstrapper HostBootstrapper
        {
            get
            {
                return (IAppBootstrapper)HostScreen;
            }
            protected set
            {
                HostScreen = HostBootstrapper;
            }
        }

        public IMessageBus HostMessageBus
        {
            get;
            protected set;
        }

        /* COOLSTUFF: Why the Screen here?
         *
         * Every RoutableViewModel has a pointer to its IScreen. This is really
         * useful in a unit test runner, because you can create a dummy screen,
         * invoke Commands / change Properties, then test to see if you navigated
         * to the correct new screen 
         */
        public ViewModelBase(IAppBootstrapper bootstrapper, IMessageBus messageBus)
        {
            HostScreen = bootstrapper;
            HostMessageBus = messageBus;
        }
    }
}
