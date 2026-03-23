using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WindowsCustomizer.Views;
using Microsoft.UI.Composition.SystemBackdrops;

namespace WindowsCustomizer
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.Title = "Windows Customizer";

            if (MicaController.IsSupported())
            {
                SystemBackdrop = new MicaBackdrop();
            }

            navView.ItemInvoked += OnNavViewItemInvoked;
            navView.SelectedItem = navView.MenuItems[0];
            contentFrame.Navigate(typeof(HomePage));
        }

        private void OnNavViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                // To-Do: Navigate to settings page
                // contentFrame.Navigate(typeof(SettingsPage));
            }
            else if (args.InvokedItemContainer != null)
            {
                var tag = args.InvokedItemContainer.Tag.ToString();
                switch (tag)
                {
                    case "Home":
                        contentFrame.Navigate(typeof(HomePage));
                        break;
                    case "Bloatware":
                        contentFrame.Navigate(typeof(BloatwarePage));
                        break;
                    case "Essentials":
                        contentFrame.Navigate(typeof(EssentialsPage));
                        break;
                    case "Explorer":
                        contentFrame.Navigate(typeof(ExplorerPage));
                        break;
                    case "Search":
                        contentFrame.Navigate(typeof(SearchPage));
                        break;
                    case "Services":
                        contentFrame.Navigate(typeof(ServicesPage));
                        break;
                }
            }
        }
    }
}

