using InterfacesProject;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace ModuleAProject
{
    public class ModuleA : IModule
    {
        private IUnityContainer _unityContainer;
        private IRegionManager _regionManager;

        public ModuleA(IUnityContainer unityContainer, IRegionManager regionManager)
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
            _regionManager.RegisterViewWithRegion("ListRegion", typeof (ModuleAViewOne));
            _regionManager.RegisterViewWithRegion("ListRegion", typeof (ModuleAViewTwo));
        }

        private void RegisterServices()
        {
            _unityContainer.RegisterType<ITextService, TextService>();
        }
    }
}
