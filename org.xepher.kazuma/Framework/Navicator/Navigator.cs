using Framework.Container;
using Framework.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Navicator
{
    public sealed class Navigator : INavigator
    {
        private readonly ISerializer serializer;
        private readonly IContainer container;
        private readonly Microsoft.Phone.Controls.PhoneApplicationFrame frame;

        public Navigator(
            ISerializer serializer,
            IContainer container, 
            Microsoft.Phone.Controls.PhoneApplicationFrame frame)
        {
            this.serializer = serializer;
            this.container = container;
            this.frame = frame;
        }

        public void NavigateToViewModel<TViewModel>(object parameter = null)
        {
            var viewType = ResolveViewType<TViewModel>();

            if (parameter != null)
            {
                this.frame.Navigate(ResolveViewUri(viewType, parameter));
            }
            else
            {
                this.frame.Navigate(ResolveViewUri(viewType));
            }
        }

        public void GoBack()
        {
            this.frame.GoBack();
        }

        public bool CanGoBack
        {
            get
            {
                return this.frame.CanGoBack;
            }
        }

        private static Type ResolveViewType<TViewModel>()
        {
            var viewModelType = typeof(TViewModel);

            var viewName = viewModelType.AssemblyQualifiedName.Replace(
                viewModelType.Name,
                viewModelType.Name.Replace("ViewModel", "Page"));

            return Type.GetType(viewName.Replace("Model", string.Empty));
        }

        private Uri ResolveViewUri(Type viewType, object parameter = null)
        {
            var queryString = string.Empty;
            if (parameter != null)
            {
                var serializedParameter = this.serializer.Serialize(parameter);
                queryString = string.Format("?parameter={0}", serializedParameter);
            }

            var match = System.Text.RegularExpressions.Regex.Match(viewType.FullName, @"\.Views.*");
            if (match == null || match.Captures.Count == 0)
            {
                throw new ArgumentException("Views must exist in Views namespace.");
            }
            var path = match.Captures[0].Value.Replace('.', '/');

            return new Uri(string.Format("{0}.xaml{1}", path, queryString), UriKind.Relative);
        }

        public void RemoveBackEntry()
        {
            this.frame.RemoveBackEntry();
        }
    }
}
