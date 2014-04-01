using InterfacesProject;

namespace ModuleBProject
{
    public class TextService : ITextService
    {
        public string GetText()
        {
            return "Hello World";
        }
    }
}
