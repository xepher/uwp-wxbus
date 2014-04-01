using InterfacesProject;

namespace ModuleAProject.ViewModels
{
    public class ModuleAViewOneViewModel
    {
        private ITextService _textService;
        public ModuleAViewOneViewModel(ITextService textService)
        {
            _textService = textService;
        }

        public string Text
        {
            get { return _textService.GetText(); }
        }
    }
}
