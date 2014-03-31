using InterfacesProject;

namespace ModuleAProject
{
    public class TextService : ITextService
    {
        public string GetText()
        {
            return "Hello World";
        }
    }
}
