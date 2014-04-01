using InterfacesProject;

namespace ModuleAProject.ViewModels
{
    public class ModuleAViewTwoViewModel
    {
        private ITextService _textService;
        public ModuleAViewTwoViewModel(ITextService textService)
        {
            _textService = textService;
        }

        public int Text
        {
            get { return _textService.GetText().Length; }
        }
    }
}
