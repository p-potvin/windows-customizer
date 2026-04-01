using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WindowsCustomizer.Models;
using System.Linq;
using Windows.UI;

namespace WindowsCustomizer.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            ThemeSelector.ItemsSource = VaultThemeManager.Themes;
            
            // Try to set current theme
            if (Application.Current.Resources.ContainsKey("VaultPrimaryColor"))
            {
                var currentPrimary = (Color)Application.Current.Resources["VaultPrimaryColor"];
                var currentTheme = VaultThemeManager.Themes.FirstOrDefault(t => t.Primary == currentPrimary);
                if (currentTheme != null)
                {
                    ThemeSelector.SelectedItem = currentTheme;
                }
            }
        }

        private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeSelector.SelectedItem is VaultTheme theme)
            {
                var res = Application.Current.Resources;
                res["VaultPrimaryColor"] = theme.Primary;
                res["VaultAccentColor"] = theme.Accent;
                res["SystemAccentColor"] = theme.Accent;
                
                // Update brushes if they exist
                if (res["VaultPrimaryBrush"] is SolidColorBrush primaryBrush)
                    primaryBrush.Color = theme.Primary;
                
                if (res["VaultAccentBrush"] is SolidColorBrush accentBrush)
                    accentBrush.Color = theme.Accent;
                
                if (res["SystemAccentColorBrush"] is SolidColorBrush systemAccentBrush)
                    systemAccentBrush.Color = theme.Accent;

                // Apply theme to the application resources Window level if possible, 
                // but since we are in a Page, we should use the Window from the App class or XamlRoot.
                if (this.XamlRoot?.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = theme.Mode.ToLower() == "dark" ? ElementTheme.Dark : ElementTheme.Light;
                }
            }
        }
    }
}