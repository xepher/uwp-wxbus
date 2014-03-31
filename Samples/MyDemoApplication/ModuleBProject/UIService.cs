using System.Windows;
using System.Windows.Controls;
using InterfacesProject;

namespace ModuleBProject
{
    public class UIService : IUIService
    {
        private ITextService _textService;

        public UIService(ITextService textService)
        {
            _textService = textService;
        }

        public UIElement GetUI()
        {
            return new TextBlock()
            {
                Text = _textService.GetText()
            };
        }
    }
}
