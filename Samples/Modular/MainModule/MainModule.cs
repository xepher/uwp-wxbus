using System.Diagnostics;
using Interface;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace MainModule
{
    [Module(ModuleName = "MainModule", OnDemand = true)]
    public class MainModule : IModule
    {
        private IUnityContainer _container;
        private IServiceLocator _serviceLocator;
        private ICalculator _calculator;
        public ICalculator Calculator
        {
            get { return _calculator; }
        }

        public MainModule(IUnityContainer container,IServiceLocator serviceLocator)
        {
            _container = container;
            _serviceLocator = serviceLocator;
        }

        public void Initialize()
        {
            Debug.WriteLine(">>>Initialize MainModule");

            _container.RegisterType<ICalculator, Calculator>();
            _calculator = _serviceLocator.GetInstance<ICalculator>();

            _calculator.Run();
        }
    }
}
