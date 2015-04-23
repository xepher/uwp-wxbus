using Org.Xepher.Kazuma.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class IAPViewModel : ViewModelBase
    {
        public IAPViewModel(IScreen screen, IMessageBus messageBus)
            : base(screen, messageBus)
        {
            base.PathSegment = Constants.PATH_SEGMENT_IAP;
        }
    }
}
