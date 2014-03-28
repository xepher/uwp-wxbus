using System;
using System.Configuration;
using Interface;
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
            IUnityContainer container = new UnityContainer();

            UnityConfigurationSection configSection =
                (UnityConfigurationSection)ConfigurationManager.GetSection("unity");

            //configSection.Containers.Default.Configure(container);
            configSection.Configure(container);

            //container.LoadConfiguration();
            container.RegisterInstance<IServiceLocator>(new UnityServiceLocatorAdapter(container));

            ICalculator calculator = container.Resolve<ICalculator>();
            Console.WriteLine(calculator.add(10, 20));
        }
    }
}
