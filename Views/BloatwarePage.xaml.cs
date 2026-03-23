using Microsoft.UI.Xaml.Controls;
using WindowsCustomizer.ViewModels;

namespace WindowsCustomizer.Views
{
    public sealed partial class BloatwarePage : Page
    {
        public BloatwareViewModel ViewModel { get; }

        public BloatwarePage()
        {
            this.InitializeComponent();
            ViewModel = new BloatwareViewModel();
            this.DataContext = ViewModel;
        }
    }
}
