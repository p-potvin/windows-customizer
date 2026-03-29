using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using WindowsCustomizer.Common;
using WindowsCustomizer.Models;

namespace WindowsCustomizer.ViewModels;

public class SearchViewModel : ObservableObject
{
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ObservableCollection<ExplorerSetting> Settings { get; } = new();
    public ICommand ApplyChangesCommand { get; }

    public SearchViewModel()
    {
        ApplyChangesCommand = new AsyncRelayCommand(ApplyChangesAsync);
        LoadSettings();
    }

    private void LoadSettings()
    {
        var settings = new[]
        {
            new ExplorerSetting
            {
                Name = "Disable Search Suggestions (Bing)",
                Description = "Remove internet search results (Bing) and cloud content from the Windows search bar.",
                RegistryKey = @"Software\Policies\Microsoft\Windows\Explorer",
                RegistryValue = "DisableSearchBoxSuggestions",
                EnabledValue = 1,
                DisabledValue = 0
            },
            new ExplorerSetting
            {
                Name = "Disable Bing Search",
                Description = "Further disable Bing integration in Windows Search (Search UI).",
                RegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Search",
                RegistryValue = "BingSearchEnabled",
                EnabledValue = 0,
                DisabledValue = 1
            },
            new ExplorerSetting
            {
                Name = "Disable Cortana",
                Description = "Disable Cortana as a digital assistant.",
                RegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Search",
                RegistryValue = "CortanaConsent",
                EnabledValue = 0,
                DisabledValue = 1
            },
            new ExplorerSetting
            {
                Name = "Hide Search Highlight (Doodle)",
                Description = "Hide the 'Search Highlights' content (doodles) in the search box.",
                RegistryKey = @"Software\Policies\Microsoft\Windows\Windows Search",
                RegistryValue = "EnableDynamicContentInSearchBox",
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
                    using var key = Registry.CurrentUser.CreateSubKey(setting.RegistryKey, true);
                    if (setting.IsEnabled)
                    {
                        key.SetValue(setting.RegistryValue, setting.EnabledValue, RegistryValueKind.DWord);
                    }
                    else
                    {
                        key.SetValue(setting.RegistryValue, setting.DisabledValue, RegistryValueKind.DWord);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error applying {setting.Name}: {ex.Message}");
                }
            }
        });
        IsLoading = false;
    }
}
