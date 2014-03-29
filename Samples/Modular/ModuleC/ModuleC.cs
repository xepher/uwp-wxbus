using System.Diagnostics;
using Interface;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace ModuleC
{
    [Module(ModuleName = "ModuleC", OnDemand = true)]
    public class ModuleC : IModule
    {
        private IUnityContainer _container;

        public ModuleC(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            Debug.WriteLine(">>>Initialize ModuleC");
            _container.RegisterType<ICalculatorMul, CalculatorMul>();
        }
    }
}
