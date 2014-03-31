using System;
using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;

namespace MyDemoApplication
{
    public class MyBootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return null;
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return
                Microsoft.Practices.Prism.Modularity.ModuleCatalog.CreateFromXaml(new Uri("modulecatalog.xaml",
                    UriKind.Relative));
        }
    }
}
