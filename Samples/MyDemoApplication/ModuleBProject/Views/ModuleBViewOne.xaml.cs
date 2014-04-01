using System.Windows.Controls;
using ModuleBProject.ViewModels;

namespace ModuleBProject
{
    public partial class ModuleBViewOne : UserControl
    {
        public ModuleBViewOne(ModuleBViewOneViewModel model)
        {
            InitializeComponent();

            this.Loaded+= (s, e) =>
            {
                this.DataContext = model;
            };
        }
    }
}
