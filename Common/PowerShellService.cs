using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Threading.Tasks;
using WindowsCustomizer.Models;

namespace WindowsCustomizer.Services;

public static class PowerShellService
{
    public static async Task<List<AppxPackage>> GetAppxPackagesAsync()
    {
        var packages = new List<AppxPackage>();

        await Task.Run(() =>
        {
            using PowerShell ps = PowerShell.Create();
            // Get packages for all users
            ps.AddCommand("Get-AppxPackage");
            ps.AddParameter("AllUsers");

            try
            {
                Collection<PSObject> results = ps.Invoke();

                foreach (PSObject result in results)
                {
                    // Filter out system/framework packages that shouldn't be removed by a typical user
                    bool isFramework = result.Properties["IsFramework"]?.Value as bool? ?? false;
                    var signatureKind = result.Properties["SignatureKind"]?.Value?.ToString();
                    if (isFramework || signatureKind == "System")
                    {
                        continue;
                    }

                    packages.Add(new AppxPackage
                    {
                        Name = result.Properties["Name"]?.Value?.ToString() ?? "N/A",
                        PackageFullName = result.Properties["PackageFullName"]?.Value?.ToString() ?? "N/A",
                        Publisher = result.Properties["Publisher"]?.Value?.ToString() ?? "N/A",
                        Version = result.Properties["Version"]?.Value?.ToString() ?? "N/A",
                    });
                }
            }
            catch (Exception) { /* In a real app, this should be logged */ }
        });

        return packages;
    }

    public static async Task<bool> RemoveAppxPackageAsync(string packageFullName)
    {
        bool success = false;
        await Task.Run(() =>
        {
            using PowerShell ps = PowerShell.Create();
            // This command requires administrator privileges.
            ps.AddCommand("Remove-AppxPackage");
            ps.AddParameter("Package", packageFullName);
            ps.AddParameter("AllUsers");

            try
            {
                ps.Invoke();
                if (ps.HadErrors)
                {
                    foreach (var error in ps.Streams.Error)
                    {
                        System.Diagnostics.Debug.WriteLine($"PowerShell Error removing {packageFullName}: {error}");
                    }
                    success = false;
                }
                else
                {
                    success = true;
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"PowerShell Exception removing {packageFullName}: {ex.Message}"); }
        });
        return success;
    }

    public static async Task<List<ServicePackage>> GetServicesAsync()
    {
        var services = new List<ServicePackage>();

        await Task.Run(() =>
        {
            using PowerShell ps = PowerShell.Create();
            ps.AddCommand("Get-Service");

            try
            {
                Collection<PSObject> results = ps.Invoke();

                foreach (PSObject result in results)
                {
                    services.Add(new ServicePackage
                    {
                        Name = result.Properties["Name"]?.Value?.ToString() ?? "N/A",
                        DisplayName = result.Properties["DisplayName"]?.Value?.ToString() ?? "N/A",
                        Status = result.Properties["Status"]?.Value?.ToString() ?? "N/A",
                        StartType = result.Properties["StartType"]?.Value?.ToString() ?? "N/A",
                    });
                }
            }
            catch (Exception) { }
        });

        return services;
    }

    public static async Task<bool> SetServiceStartTypeAsync(string name, string startType)
    {
        bool success = false;
        await Task.Run(() =>
        {
            using PowerShell ps = PowerShell.Create();
            ps.AddCommand("Set-Service");
            ps.AddParameter("Name", name);
            ps.AddParameter("StartupType", startType);

            try
            {
                ps.Invoke();
                success = !ps.HadErrors;
            }
            catch (Exception) { }
        });
        return success;
    }

    public static async Task<List<StartupPackage>> GetStartupPackagesAsync()
    {
        var startupItems = new List<StartupPackage>();

        await Task.Run(() =>
        {
            using PowerShell ps = PowerShell.Create();
            ps.AddScript("Get-CimInstance Win32_StartupCommand | Select-Object Name, Command, User, Location");

            try
            {
                Collection<PSObject> results = ps.Invoke();

                foreach (PSObject result in results)
                {
                    startupItems.Add(new StartupPackage
                    {
                        Name = result.Properties["Name"]?.Value?.ToString() ?? "N/A",
                        Command = result.Properties["Command"]?.Value?.ToString() ?? "N/A",
                        User = result.Properties["User"]?.Value?.ToString() ?? "N/A",
                        Location = result.Properties["Location"]?.Value?.ToString() ?? "N/A",
                    });
                }
            }
            catch (Exception) { }
        });

        return startupItems;
    }

    public static async Task<bool> DisableStartupPackageAsync(string name, string location)
    {
        bool success = false;
        await Task.Run(() =>
        {
            using PowerShell ps = PowerShell.Create();
            string script = "";

            if (location.StartsWith("HKLM", StringComparison.OrdinalIgnoreCase))
            {
                script = $"Remove-ItemProperty -Path 'Registry::{location}' -Name '{name}' -ErrorAction SilentlyContinue";
            }
            else if (location.StartsWith("Startup", StringComparison.OrdinalIgnoreCase))
            {
                // Likely a shortcut in a startup folder.
                script = $"Get-ChildItem -Path $env:APPDATA\\Microsoft\\Windows\\'Start Menu'\\Programs\\Startup | Where-Object {{ $_.Name -like '*{name}*' }} | Remove-Item -ErrorAction SilentlyContinue";
            }
            else
            {
                // Generalized registry removal for other locations
                script = $"Remove-ItemProperty -Path 'Registry::{location}' -Name '{name}' -ErrorAction SilentlyContinue";
            }

            ps.AddScript(script);

            try
            {
                ps.Invoke();
                success = !ps.HadErrors;
            }
            catch (Exception) { }
        });
        return success;
    }

    public static async Task<bool> InstallWingetPackageAsync(string wingetId)
    {
        bool success = false;
        await Task.Run(() =>
        {
            using PowerShell ps = PowerShell.Create();
            // --silent --accept-source-agreements --accept-package-agreements
            ps.AddScript($"winget install --id {wingetId} --silent --accept-source-agreements --accept-package-agreements");

            try
            {
                ps.Invoke();
                success = !ps.HadErrors;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error installing {wingetId}: {ex.Message}");
            }
        });
        return success;
    }
}