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
}