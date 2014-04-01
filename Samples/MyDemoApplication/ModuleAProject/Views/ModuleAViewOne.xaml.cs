using System.Windows.Controls;
using ModuleAProject.ViewModels;

namespace ModuleAProject
{
    public partial class ModuleAViewOne : UserControl
    {
        public ModuleAViewOne(ModuleAViewOneViewModel model)
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                this.DataContext = model;
            };
        }
    }
}
