using System;
using System.Linq;
using Xunit;
using WindowsCustomizer.Models;
using WindowsCustomizer.ViewModels;

namespace WindowsCustomizer.Tests
{
    public class ExplorerViewModelTests
    {
        [Fact]
        public void ExplorerViewModel_LoadsAllExpectedSettings()
        {
            // Arrange
            var viewModel = new ExplorerViewModel();

            // Act
            var settings = viewModel.Settings;

            // Assert
            Assert.NotNull(settings);
            Assert.NotEmpty(settings);

            // Verify our newly added settings are present
            Assert.Contains(settings, s => s.Name == "Hide Libraries from Sidebar");
            Assert.Contains(settings, s => s.Name == "Hide Gallery from Sidebar");
            Assert.Contains(settings, s => s.Name == "Show Hidden Files");
        }

        [Fact]
        public void ExplorerViewModel_SettingsHaveValidRegistryData()
        {
            // Arrange
            var viewModel = new ExplorerViewModel();

            // Act
            var settings = viewModel.Settings;

            // Assert
            foreach (var setting in settings)
            {
                Assert.False(string.IsNullOrWhiteSpace(setting.Name));
                Assert.False(string.IsNullOrWhiteSpace(setting.RegistryKey));
                Assert.NotNull(setting.RegistryValue); // Registry value could technically be empty string for default value.
            }
        }
    }
}
