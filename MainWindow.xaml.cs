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
        private MicaController _micaController;
        private SystemBackdropConfiguration _backdropConfiguration;

        public MainWindow()
        {
            this.InitializeComponent();
            Title = "Windows Customizer";
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(NavView); // Use NavView as title bar
            TrySetMicaBackdrop();
        }

        private bool TrySetMicaBackdrop()
        {
            if (MicaController.IsSupported())
            {
                _micaController = new MicaController();
                _backdropConfiguration = new SystemBackdropConfiguration();

                // Hook up the theme change event
                ((FrameworkElement)this.Content).ActualThemeChanged += (s, e) =>
                {
                    if (_backdropConfiguration != null)
                    {
                        SetBackdropTheme();
                    }
                };

                // Initial theme setup
                SetBackdropTheme();

                _micaController.SetSystemBackdropConfiguration(_backdropConfiguration);
                _micaController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());

                // Make the window transparent
                ((FrameworkElement)this.Content).Background = new SolidColorBrush(Colors.Transparent);
                
                return true;
            }

            return false; // Mica is not supported on this system
        }

        private void SetBackdropTheme()
        {
            switch (((FrameworkElement)this.Content).ActualTheme)
            {
                case ElementTheme.Dark:
                    _backdropConfiguration.Theme = SystemBackdropTheme.Dark;
                    break;
                case ElementTheme.Light:
                    _backdropConfiguration.Theme = SystemBackdropTheme.Light;
                    break;
                case ElementTheme.Default:
                    _backdropConfiguration.Theme = SystemBackdropTheme.Default;
                    break;
            }
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
                // Add other specific pages here if they exist
                // case "installer":
                //     pageType = typeof(Views.InstallerPage);
                //     break;
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