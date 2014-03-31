using InterfacesProject;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace ModuleAProject
{
    public class ModuleA : IModule
    {
        private IUnityContainer _unityContainer;

        public ModuleA(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            _unityContainer.RegisterType<ITextService, TextService>();
        }
    }
}
