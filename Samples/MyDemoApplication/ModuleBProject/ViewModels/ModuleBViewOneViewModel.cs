using InterfacesProject;

namespace ModuleBProject.ViewModels
{
    public class ModuleBViewOneViewModel
    {
        private ITextService _textService;

        public ModuleBViewOneViewModel(ITextService textService)
        {
            _textService = textService;
        }

        public string Text
        {
            get { return _textService.GetText().Split(' ')[0]; }
        }
    }
}
