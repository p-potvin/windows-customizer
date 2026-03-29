using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;
using Windows.UI;
using WinRT; // For As<I>

namespace WindowsCustomizer
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Title = "Windows Customizer";
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(NavView); // Use NavView as title bar

            this.SystemBackdrop = new MicaBackdrop();
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the initial selected item
            NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().First();
            ContentFrame.Navigate(typeof(Views.HomePage));
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(Views.SettingsPage));
                return;
            }

            var item = args.InvokedItemContainer as NavigationViewItem;
            if (item == null) return;

            var tag = item.Tag.ToString();
            Type pageType;

            switch (tag)
            {
                case "home":
                    pageType = typeof(Views.HomePage);
                    break;
                case "bloatware":
                    pageType = typeof(Views.BloatwareRemovalPage);
                    break;
                case "installer":
                    pageType = typeof(Views.EssentialsPage);
                    break;
                case "explorer":
                    pageType = typeof(Views.ExplorerPage);
                    break;
                case "search":
                    pageType = typeof(Views.SearchPage);
                    break;
                case "services":
                    pageType = typeof(Views.InvasiveServicesPage);
                    break;
                default:
                    pageType = typeof(Views.PlaceholderPage);
                    break;
            }

            if (ContentFrame.CurrentSourcePageType != pageType)
            {
                ContentFrame.Navigate(pageType, item.Content.ToString());
            }
        }
    }
}