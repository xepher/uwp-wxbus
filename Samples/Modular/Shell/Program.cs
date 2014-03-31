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
            //#region Initialize Settings manually
            //IUnityContainer _container = new UnityContainer();

            //#region Unity Configuration Settings
            //UnityConfigurationSection configSection =
            //    (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

            //configSection.Configure(_container);
            //#endregion

            //_container.RegisterInstance<IServiceLocator>(new UnityServiceLocatorAdapter(_container));

            //#region ModuleCatelog Register
            //_container.RegisterType<IModuleInitializer, ModuleInitializer>();

            //TextLogger logger = new TextLogger();
            //_container.RegisterInstance<ILoggerFacade>(logger);

            //_container.RegisterType<IModuleManager, ModuleManager>();

            //ConfigurationModuleCatalog configurationModuleCatalog = new ConfigurationModuleCatalog();
            //_container.RegisterInstance<IModuleCatalog>(configurationModuleCatalog);

            //IModuleManager _moduleManager = _container.Resolve<IModuleManager>();
            //_moduleManager.Run();
            //#endregion
            //#endregion

            #region Initialize Settings Use Bootstrapper
            CalculatorBootstrapper calculatorBootstrapper = new CalculatorBootstrapper();
            calculatorBootstrapper.Run();
            #endregion
        }
    }
}
