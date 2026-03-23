using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace WindowsCustomizer.Views
{
    public sealed partial class PlaceholderPage : Page
    {
        public PlaceholderPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string title)
            {
                PageTitle.Text = $"{title} Page";
            }
        }
    }
}