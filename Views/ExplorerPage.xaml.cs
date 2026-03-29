using Microsoft.UI.Xaml.Controls;
using WindowsCustomizer.ViewModels;

namespace WindowsCustomizer.Views
{
    public sealed partial class ExplorerPage : Page
    {
        public ExplorerViewModel ViewModel { get; } = new();

        public ExplorerPage()
        {
            this.InitializeComponent();
        }
    }
}
