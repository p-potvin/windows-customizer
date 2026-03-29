using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WindowsCustomizer.ViewModels;

namespace WindowsCustomizer.Views
{
    public sealed partial class BloatwareRemovalPage : Page
    {
        public BloatwareRemovalViewModel ViewModel { get; }

        public BloatwareRemovalPage()
        {
            this.InitializeComponent();
            ViewModel = new BloatwareRemovalViewModel();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.LoadAllAsync();
        }
    }
}