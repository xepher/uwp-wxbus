using Org.Xepher.Kazuma.Common;
using Org.Xepher.Kazuma.Utils;
using ReactiveUI;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class IAPViewModel : ViewModelBase
    {
        public IAPViewModel(IAppBootstrapper bootstrapper, IMessageBus messageBus)
            : base(bootstrapper, messageBus)
        {
            base.PathSegment = Constants.PATH_SEGMENT_IAP;
        }
    }
}
