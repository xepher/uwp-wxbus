using System.Windows.Controls;
using ModuleAProject.ViewModels;

namespace ModuleAProject
{
    public partial class ModuleAViewTwo : UserControl
    {
        public ModuleAViewTwo(ModuleAViewTwoViewModel model)
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                this.DataContext = model;
            };
        }
    }
}
