using InterfacesProject;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace ModuleBProject
{
    public class ModuleB : IModule
    {
        private IUnityContainer _unityContainer;
        private IRegionManager _regionManager;

        public ModuleB(IUnityContainer unityContainer, IRegionManager regionManager)
        {
            _unityContainer = unityContainer;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            RegisterServices();
            RegisterViews();
        }

        private void RegisterViews()
        {
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(ModuleBViewOne));
        }

        private void RegisterServices()
        {
            _unityContainer.RegisterType<ITextService, TextService>();
        }
    }
}
