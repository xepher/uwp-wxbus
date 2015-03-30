using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(IScreen screen)
            : base(screen)
        {
            base.PathSegment = "Settings";
        }
    }
}
