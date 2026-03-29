using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using WindowsCustomizer.Common;
using WindowsCustomizer.Models;

namespace WindowsCustomizer.ViewModels;

public class ExplorerViewModel : ObservableObject
{
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ObservableCollection<ExplorerSetting> Settings { get; } = new();
    public ICommand ApplyChangesCommand { get; }

    public ExplorerViewModel()
    {
        ApplyChangesCommand = new AsyncRelayCommand(ApplyChangesAsync);
        LoadSettings();
    }

    private void LoadSettings()
    {
        // Define settings
        var settings = new[]
        {
            new ExplorerSetting
            {
                Name = "Show Hidden Files",
                Description = "Show files and folders that have the hidden attribute set.",
                RegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                RegistryValue = "Hidden",
                EnabledValue = 1,
                DisabledValue = 2
            },
            new ExplorerSetting
            {
                Name = "Show File Extensions",
                Description = "Display file extension suffixes. (e.g., .txt, .exe)",
                RegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                RegistryValue = "HideFileExt",
                EnabledValue = 0,
                DisabledValue = 1
            },
            new ExplorerSetting
            {
                Name = "Classic Context Menu (Windows 11)",
                Description = "Restore the old right-click menu instead of the Win11 version.",
                RegistryKey = @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32",
                RegistryValue = "", // Default value
                EnabledValue = "", // Set to empty string
                DisabledValue = null // Deleting the key/value for default behavior
            },
            new ExplorerSetting
            {
                Name = "Hide Sync Notifications",
                Description = "Disable provider notifications (OneDrive/sync ads) in Explorer.",
                RegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                RegistryValue = "ShowSyncNotifications",
                EnabledValue = 0,
                DisabledValue = 1
            }
        };

        foreach (var s in settings)
        {
            s.IsEnabled = GetCurrentState(s);
            Settings.Add(s);
        }
    }

    private bool GetCurrentState(ExplorerSetting setting)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(setting.RegistryKey);
            if (key == null) return false;

            var val = key.GetValue(setting.RegistryValue);
            if (val == null) return false;

            return val.ToString() == setting.EnabledValue.ToString();
        }
        catch { return false; }
    }

    private async Task ApplyChangesAsync()
    {
        IsLoading = true;
        await Task.Run(() =>
        {
            foreach (var setting in Settings)
            {
                try
                {
                    if (setting.IsEnabled)
                    {
                        if (setting.Name.Contains("Classic Context Menu"))
                        {
                            using var key = Registry.CurrentUser.CreateSubKey(setting.RegistryKey, true);
                            key.SetValue(setting.RegistryValue, setting.EnabledValue);
                        }
                        else
                        {
                            using var key = Registry.CurrentUser.CreateSubKey(setting.RegistryKey, true);
                            key.SetValue(setting.RegistryValue, setting.EnabledValue, RegistryValueKind.DWord);
                        }
                    }
                    else
                    {
                        if (setting.Name.Contains("Classic Context Menu"))
                        {
                            // Remove the key to disable classic context menu (restore default Win11 behaviour)
                            Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}", false);
                        }
                        else
                        {
                            using var key = Registry.CurrentUser.CreateSubKey(setting.RegistryKey, true);
                            key.SetValue(setting.RegistryValue, setting.DisabledValue, RegistryValueKind.DWord);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error applying {setting.Name}: {ex.Message}");
                }
            }

            // Restart explorer to apply changes (ideally)
            // For now just notify user or let it happen eventually.
        });
        IsLoading = false;
    }
}
