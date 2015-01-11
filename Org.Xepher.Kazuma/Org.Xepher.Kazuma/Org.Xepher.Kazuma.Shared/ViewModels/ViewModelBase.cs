using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.ViewModels
{
    public abstract class ViewModelBase : Screen
    {
        private readonly INavigationService navigationService;

        protected ViewModelBase(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public void GoBack()
        {
            navigationService.GoBack();
        }

        public bool CanGoBack
        {
            get
            {
                return navigationService.CanGoBack;
            }
        }
    }
}
