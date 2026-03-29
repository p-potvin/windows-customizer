using System;
using System.Linq;
using Xunit;
using WindowsCustomizer.Models;
using WindowsCustomizer.ViewModels;

namespace WindowsCustomizer.Tests
{
    public class InvasiveServicesViewModelTests
    {
        [Fact]
        public void InvasiveServicesViewModel_LoadServices_PopulatesCorrectly()
        {
            // Arrange
            var viewModel = new InvasiveServicesViewModel();

            // Act
            var services = viewModel.Services;

            // Assert
            Assert.NotNull(services);
            Assert.Equal(4, services.Count); // Telemetry, OneDrive, Hosts block, Spotlight
            
            // Validate expected invasive services
            Assert.Contains(services, s => s.Name == "Windows Telemetry (DiagTrack)");
            Assert.Contains(services, s => s.Name == "OneDrive Integration");
            Assert.Contains(services, s => s.Name == "DNS Telemetry Blocklist (Hosts File)");
            Assert.Contains(services, s => s.Name == "Windows Spotlight");
        }

        [Fact]
        public void InvasiveService_Model_StateUpdates_TriggerPropertyChanged()
        {
            // Arrange
            var service = new InvasiveService { Name = "Test Service", Description = "Test" };
            bool eventFired = false;
            
            service.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(InvasiveService.IsEnabled))
                {
                    eventFired = true;
                }
            };

            // Act
            service.IsEnabled = !service.IsEnabled; // toggle state

            // Assert
            Assert.True(eventFired);
        }
    }
}
