using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsCustomizer.Common;
using WindowsCustomizer.Models;
using WindowsCustomizer.Services;
using Microsoft.Win32;

namespace WindowsCustomizer.ViewModels;

public class InvasiveServicesViewModel : ObservableObject
{
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ObservableCollection<InvasiveService> Services { get; } = new();
    public ICommand ApplyChangesCommand { get; }

    public InvasiveServicesViewModel()
    {
        ApplyChangesCommand = new AsyncRelayCommand(ApplyChangesAsync);
        LoadServices();
    }

    private void LoadServices()
    {
        var services = new[]
        {
            new InvasiveService
            {
                Name = "Windows Telemetry (DiagTrack)",
                Description = "Collects system info and user behavior and sends it to Microsoft.",
            },
            new InvasiveService
            {
                Name = "OneDrive Integration",
                Description = "Native file sync that is tightly coupled with Explorer.",
            },
            new InvasiveService
            {
                Name = "DNS Telemetry Blocklist (Hosts File)",
                Description = "Block known Microsoft telemetry datacollection endpoints via hosts file mapping.",
            },
            new InvasiveService
            {
                Name = "Windows Spotlight",
                Description = "Downloads images and ads for the lockscreen and start menu.",
            }
        };

        foreach (var s in services)
        {
            s.IsEnabled = GetCurrentState(s);
            Services.Add(s);
        }
    }

    private bool GetCurrentState(InvasiveService service)
    {
        try
        {
            switch (service.Name)
            {
                case "Windows Telemetry (DiagTrack)":
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection"))
                    {
                        if (key != null)
                        {
                            var val = key.GetValue("AllowTelemetry");
                            if (val != null && val.ToString() == "0") return false;
                        }
                    }
                    return true;
                
                case "OneDrive Integration":
                    using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows\OneDrive"))
                    {
                        if (key != null)
                        {
                            var val = key.GetValue("DisableFileSyncNGSC");
                            if (val != null && val.ToString() == "1") return false;
                        }
                    }
                    return true;

                case "DNS Telemetry Blocklist (Hosts File)":
                    string hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
                    if (File.Exists(hostsPath))
                    {
                        var lines = File.ReadAllLines(hostsPath);
                        return !lines.Any(l => l.Contains("vortex.data.microsoft.com"));
                    }
                    return true; // Not blocked

                case "Windows Spotlight":
                    using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Policies\Microsoft\Windows\CloudContent"))
                    {
                        if (key != null)
                        {
                            var val = key.GetValue("DisableWindowsSpotlightFeatures");
                            if (val != null && val.ToString() == "1") return false;
                        }
                    }
                    return true;
            }
        }
        catch { }
        return true; // Default to enabled if unable to determine
    }

    private async Task ApplyChangesAsync()
    {
        IsLoading = true;
        await Task.Run(async () =>
        {
            foreach (var service in Services)
            {
                try
                {
                    if (service.Name == "Windows Telemetry (DiagTrack)")
                    {
                        using var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection", true);
                        key.SetValue("AllowTelemetry", service.IsEnabled ? 3 : 0, RegistryValueKind.DWord);
                        
                        // Also try to disable the service
                        if (!service.IsEnabled)
                        {
                            await PowerShellService.SetServiceStartTypeAsync("DiagTrack", "Disabled");
                            await PowerShellService.SetServiceStartTypeAsync("dmwappushservice", "Disabled");
                        }
                    }
                    else if (service.Name == "OneDrive Integration")
                    {
                        using var key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\OneDrive", true);
                        key.SetValue("DisableFileSyncNGSC", service.IsEnabled ? 0 : 1, RegistryValueKind.DWord);
                    }
                    else if (service.Name == "Windows Spotlight")
                    {
                        using var key = Registry.CurrentUser.CreateSubKey(@"Software\Policies\Microsoft\Windows\CloudContent", true);
                        key.SetValue("DisableWindowsSpotlightFeatures", service.IsEnabled ? 0 : 1, RegistryValueKind.DWord);
                    }
                    else if (service.Name == "DNS Telemetry Blocklist (Hosts File)")
                    {
                        string hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
                        var lines = File.ReadAllLines(hostsPath).ToList();
                        bool hasVortex = lines.Any(l => l.Contains("vortex.data.microsoft.com"));

                        if (!service.IsEnabled && !hasVortex) // We want to block it, but it isn't blocked yet
                        {
                            lines.Add("0.0.0.0 vortex.data.microsoft.com");
                            lines.Add("0.0.0.0 telemetry.microsoft.com");
                            lines.Add("0.0.0.0 settings-win.data.microsoft.com");
                            File.WriteAllLines(hostsPath, lines);
                        }
                        else if (service.IsEnabled && hasVortex) // We want to allow it, and it is currenly blocked
                        {
                            lines.RemoveAll(l => l.Contains("vortex.data.microsoft.com") || l.Contains("telemetry.microsoft.com") || l.Contains("settings-win.data.microsoft.com"));
                            File.WriteAllLines(hostsPath, lines);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error toggling {service.Name}: {ex.Message}");
                }
            }
        });
        IsLoading = false;
    }
}
