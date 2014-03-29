using System.Diagnostics;
using Interface;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace ModuleD
{
    [Module(ModuleName = "ModuleD", OnDemand = true)]
    public class ModuleD : IModule
    {
        private IUnityContainer _container;

        public ModuleD(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            Debug.WriteLine(">>>Initialize ModuleD");
            _container.RegisterType<ICalculatorDiv, CalculatorDiv>();
        }
    }
}
