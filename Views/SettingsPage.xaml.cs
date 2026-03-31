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
                Application.Current.Resources["VaultPrimaryColor"] = theme.Primary;
                Application.Current.Resources["VaultAccentColor"] = theme.Accent;
                
                // Update brushes
                if (Application.Current.Resources.ContainsKey("VaultPrimaryBrush") && 
                    Application.Current.Resources["VaultPrimaryBrush"] is SolidColorBrush primaryBrush)
                {
                    primaryBrush.Color = theme.Primary;
                }
                
                if (Application.Current.Resources.ContainsKey("VaultAccentBrush") && 
                    Application.Current.Resources["VaultAccentBrush"] is SolidColorBrush accentBrush)
                {
                    accentBrush.Color = theme.Accent;
                }
                
                // Request a theme change
                if (this.XamlRoot?.Content is FrameworkElement fe)
                {
                    fe.RequestedTheme = theme.Mode == "dark" ? ElementTheme.Dark : ElementTheme.Light;
                }
            }
        }
    }
}