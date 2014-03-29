using System.Diagnostics;
using Interface;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace ModuleA
{
    [Module(ModuleName = "ModuleA", OnDemand = true)]
    public class ModuleA : IModule
    {
        private IUnityContainer _container;

        public ModuleA(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            Debug.WriteLine(">>>Initialize ModuleA");
            _container.RegisterType<ICalculatorAdd, CalculatorAdd>();
        }
    }
}
