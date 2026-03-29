using Microsoft.UI.Xaml.Controls;
using WindowsCustomizer.ViewModels;

namespace WindowsCustomizer.Views
{
    public sealed partial class EssentialsPage : Page
    {
        public EssentialsViewModel ViewModel { get; }

        public EssentialsPage()
        {
            this.InitializeComponent();
            ViewModel = new EssentialsViewModel();
        }
    }
}
