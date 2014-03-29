using System.Diagnostics;
using Interface;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;

namespace MainModule
{
    [Module(ModuleName = "MainModule", OnDemand = true)]
    public class MainModule : IModule
    {
        private IServiceLocator _serviceLocator;
        private ICalculator _calculator;
        public ICalculator Calculator
        {
            get { return _calculator; }
        }

        public MainModule(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public void Initialize()
        {
            Debug.WriteLine(">>>Initialize MainModule");
            _calculator = _serviceLocator.GetInstance<ICalculator>();
        }
    }
}
