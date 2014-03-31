using System.Windows;

namespace InterfacesProject
{
    public interface ITextService
    {
        string GetText();
    }

    public interface IUIService
    {
        UIElement GetUI();
    }
}
