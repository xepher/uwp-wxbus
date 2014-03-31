using InterfacesProject;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace MyDemoApplication
{
    public class MainModule : IModule
    {
        private IUnityContainer _unityContainer;

        public MainModule(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        public void Initialize()
        {
            IUIService uiService = _unityContainer.Resolve<IUIService>();

            App.Current.RootVisual = uiService.GetUI();
        }
    }
}
