using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsCustomizer.Common;
using WindowsCustomizer.Models;
using WindowsCustomizer.Services;

namespace WindowsCustomizer.ViewModels;

public class EssentialsViewModel : ObservableObject
{
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ObservableCollection<EssentialProgram> Programs { get; } = new();
    public ICommand InstallSelectedCommand { get; }

    public EssentialsViewModel()
    {
        InstallSelectedCommand = new AsyncRelayCommand(InstallSelectedAsync, CanInstall);
        LoadPrograms();
    }

    private void LoadPrograms()
    {
        // Curated list
        var programs = new[]
        {
            new EssentialProgram { Name = "7-Zip", WingetId = "7zip.7zip", Category = "Utilities", Description = "A free open-source file archiver." },
            new EssentialProgram { Name = "Notepad++", WingetId = "Notepad++.Notepad++", Category = "Utilities", Description = "An open-source text editor with coding features." },
            new EssentialProgram { Name = "VLC Media Player", WingetId = "VideoLAN.VLC", Category = "Media", Description = "Versatile media player for all formats." },
            new EssentialProgram { Name = "Google Chrome", WingetId = "Google.Chrome", Category = "Browsers", Description = "Popular web browser from Google." },
            new EssentialProgram { Name = "Firefox", WingetId = "Mozilla.Firefox", Category = "Browsers", Description = "Free and open-source web browser." },
            new EssentialProgram { Name = "Brave Browser", WingetId = "BraveSoftware.BraveBrowser", Category = "Browsers", Description = "Privacy-focussed web browser with ad-blocking." },
            new EssentialProgram { Name = "Visual Studio Code", WingetId = "Microsoft.VisualStudioCode", Category = "Development", Description = "Lightweight but powerful source code editor." },
            new EssentialProgram { Name = "Git", WingetId = "Git.Git", Category = "Development", Description = "Free and open-source distributed version control system." },
            new EssentialProgram { Name = "Steam", WingetId = "Valve.Steam", Category = "Gaming", Description = "Ultimate platform for playing and creating games." },
            new EssentialProgram { Name = "Epic Games Launcher", WingetId = "EpicGames.EpicGamesLauncher", Category = "Gaming", Description = "Storefront and launcher for Epic Games." },
            new EssentialProgram { Name = "WinDirStat", WingetId = "WinDirStat.WinDirStat", Category = "Utilities", Description = "Disk usage statistics viewer." },
            new EssentialProgram { Name = "WizTree", WingetId = "AntibodySoftware.WizTree", Category = "Utilities", Description = "Fast disk space analyzer." }
        };

        foreach (var p in programs)
        {
            p.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(EssentialProgram.IsSelected)) (InstallSelectedCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged(); };
            Programs.Add(p);
        }
    }

    private bool CanInstall()
    {
        return Programs.Any(p => p.IsSelected);
    }

    private async Task InstallSelectedAsync()
    {
        var selected = Programs.Where(p => p.IsSelected).ToList();
        if (!selected.Any()) return;

        IsLoading = true;

        foreach (var p in selected)
        {
            await PowerShellService.InstallWingetPackageAsync(p.WingetId);
        }

        IsLoading = false;

        // Deselect installed items
        foreach (var p in selected)
        {
            p.IsSelected = false;
        }
    }
}
