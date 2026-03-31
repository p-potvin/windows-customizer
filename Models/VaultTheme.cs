using System.Collections.Generic;
using Microsoft.UI;
using Windows.UI;

namespace WindowsCustomizer.Models
{
    public class VaultTheme
    {
        public string Name { get; set; }
        public string Mode { get; set; } // "light" or "dark"
        public Color Primary { get; set; }
        public Color Accent { get; set; }

        public VaultTheme(string name, string mode, string primaryHex, string accentHex)
        {
            Name = name;
            Mode = mode;
            Primary = ParseHex(primaryHex);
            Accent = ParseHex(accentHex);
        }

        private static Color ParseHex(string hex)
        {
            hex = hex.Replace("#", "");
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(255, r, g, b);
        }
    }

    public static class VaultThemeManager
    {
        public static List<VaultTheme> Themes { get; } = new List<VaultTheme>
        {
            new VaultTheme("Vintage Velvet", "light", "#F5F5DC", "#800020"),
            new VaultTheme("Cyberpunk Cinder", "dark", "#073642", "#CB4B16"),
            new VaultTheme("Golden Slate", "dark", "#4A5459", "#D4AF37"),
            new VaultTheme("Modern Monolith", "light", "#FAF9F6", "#333333"),
            new VaultTheme("Crimson Bloom", "dark", "#8B0000", "#FFC0CB"),
            new VaultTheme("Ocean Mist", "light", "#D3D3D3", "#006994"),
            new VaultTheme("Neon Void", "dark", "#222222", "#00FFFF"),
            new VaultTheme("Royal Tangerine", "dark", "#4B0082", "#F28500"),
            new VaultTheme("Amethyst Frost", "light", "#FDFDFD", "#800080"),
        };
    }
}
