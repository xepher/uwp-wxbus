using System.Diagnostics;
using Interface;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace ModuleB
{
    [Module(ModuleName = "ModuleB", OnDemand = true)]
    public class ModuleB : IModule
    {
        private IUnityContainer _container;

        public ModuleB(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            Debug.WriteLine(">>>Initialize ModuleB");
            _container.RegisterType<ICalculatorSub, CalculatorSub>();
        }
    }
}
