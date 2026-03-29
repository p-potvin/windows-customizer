using Microsoft.UI.Xaml.Controls;
using WindowsCustomizer.ViewModels;

namespace WindowsCustomizer.Views
{
    public sealed partial class SearchPage : Page
    {
        public SearchViewModel ViewModel { get; } = new();

        public SearchPage()
        {
            this.InitializeComponent();
        }
    }
}
