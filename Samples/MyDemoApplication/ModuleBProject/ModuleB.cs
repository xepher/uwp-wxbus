using InterfacesProject;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace ModuleBProject
{
    public class ModuleB : IModule
    {
        private IUnityContainer _unityContainer;

        public ModuleB(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterType<IUIService, UIService>();
        }
    }
}
