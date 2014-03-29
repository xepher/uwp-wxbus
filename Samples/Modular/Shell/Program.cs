using System;
using System.Configuration;
using Interface;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer _container = new UnityContainer();

            #region Unity Configuration Settings
            UnityConfigurationSection configSection =
                (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

            configSection.Configure(_container);
            #endregion

            _container.RegisterInstance<IServiceLocator>(new UnityServiceLocatorAdapter(_container));

            #region ModuleCatelog Register
            _container.RegisterType<IModuleInitializer, ModuleInitializer>();

            TextLogger logger = new TextLogger();
            _container.RegisterInstance<ILoggerFacade>(logger);

            _container.RegisterType<IModuleManager, ModuleManager>();

            ConfigurationModuleCatalog configurationModuleCatalog = new ConfigurationModuleCatalog();
            _container.RegisterInstance<IModuleCatalog>(configurationModuleCatalog);

            IModuleManager _moduleManager = _container.Resolve<IModuleManager>();
            _moduleManager.Run();
            #endregion

            ICalculator _calculator = _container.Resolve<ICalculator>();

            Console.WriteLine(_calculator.Add(10, 20));
            Console.WriteLine(_calculator.Sub(10, 20));
            Console.WriteLine(_calculator.Mul(10, 20));
            Console.WriteLine(_calculator.Div(10, 20));
        }
    }
}
