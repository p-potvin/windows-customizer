using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WindowsCustomizer.Common;
using WindowsCustomizer.Models;
using WindowsCustomizer.Services;

namespace WindowsCustomizer.ViewModels;

public class BloatwareRemovalViewModel : ObservableObject
{
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool? _isAllSelected = false;

    public bool? _isAllSelectedAppx = false;
    public bool? IsAllSelectedAppx
    {
        get => _isAllSelectedAppx;
        set
        {
            if (SetProperty(ref _isAllSelectedAppx, value) && _isAllSelectedAppx.HasValue)
            {
                SelectAllAppx(_isAllSelectedAppx.Value);
            }
        }
    }

    public ObservableCollection<AppxPackage> AppxPackages { get; } = new();
    public ObservableCollection<ServicePackage> Services { get; } = new();
    public ObservableCollection<StartupPackage> StartupItems { get; } = new();

    public ICommand UninstallSelectedCommand { get; }
    public ICommand DisableSelectedServicesCommand { get; }
    public ICommand RemoveSelectedStartupCommand { get; }

    public BloatwareRemovalViewModel()
    {
        UninstallSelectedCommand = new AsyncRelayCommand(UninstallSelectedAsync, () => AppxPackages.Any(p => p.IsSelected));
        DisableSelectedServicesCommand = new AsyncRelayCommand(DisableSelectedServicesAsync, () => Services.Any(p => p.IsSelected));
        RemoveSelectedStartupCommand = new AsyncRelayCommand(RemoveSelectedStartupAsync, () => StartupItems.Any(p => p.IsSelected));
    }

    public async Task LoadAllAsync()
    {
        if (IsLoading) return;
        IsLoading = true;

        await Task.WhenAll(LoadPackagesAsync(), LoadServicesAsync(), LoadStartupAsync());

        IsLoading = false;
    }

    private async Task LoadPackagesAsync()
    {
        AppxPackages.Clear();
        var packages = await PowerShellService.GetAppxPackagesAsync();
        packages.Sort((x, y) => string.Compare(x.Name, y.Name, System.StringComparison.Ordinal));
        foreach (var package in packages)
        {
            package.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(AppxPackage.IsSelected)) UpdateAppxSelection(); };
            AppxPackages.Add(package);
        }
    }

    private async Task LoadServicesAsync()
    {
        Services.Clear();
        var services = await PowerShellService.GetServicesAsync();
        services = services.OrderBy(s => s.DisplayName).ToList();
        foreach (var service in services)
        {
            service.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(ServicePackage.IsSelected)) (DisableSelectedServicesCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged(); };
            Services.Add(service);
        }
    }

    private async Task LoadStartupAsync()
    {
        StartupItems.Clear();
        var startup = await PowerShellService.GetStartupPackagesAsync();
        foreach (var item in startup)
        {
            item.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(StartupPackage.IsSelected)) (RemoveSelectedStartupCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged(); };
            StartupItems.Add(item);
        }
    }

    private async Task UninstallSelectedAsync()
    {
        var selected = AppxPackages.Where(p => p.IsSelected).ToList();
        if (!selected.Any()) return;

        IsLoading = true;
        foreach (var package in selected)
        {
            await PowerShellService.RemoveAppxPackageAsync(package.PackageFullName);
        }
        await LoadPackagesAsync();
        IsLoading = false;
    }

    private async Task DisableSelectedServicesAsync()
    {
        var selected = Services.Where(p => p.IsSelected).ToList();
        if (!selected.Any()) return;

        IsLoading = true;
        foreach (var service in selected)
        {
            await PowerShellService.SetServiceStartTypeAsync(service.Name, "Disabled");
        }
        await LoadServicesAsync();
        IsLoading = false;
    }

    private async Task RemoveSelectedStartupAsync()
    {
        var selected = StartupItems.Where(p => p.IsSelected).ToList();
        if (!selected.Any()) return;

        IsLoading = true;
        foreach (var item in selected)
        {
            await PowerShellService.DisableStartupPackageAsync(item.Name, item.Location);
        }
        await LoadStartupAsync();
        IsLoading = false;
    }

    private void SelectAllAppx(bool select)
    {
        foreach (var package in AppxPackages) package.IsSelected = select;
    }

    private void UpdateAppxSelection()
    {
        (UninstallSelectedCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        bool allSelected = AppxPackages.Any() && AppxPackages.All(p => p.IsSelected);
        bool noneSelected = !AppxPackages.Any() || AppxPackages.All(p => !p.IsSelected);
        _isAllSelectedAppx = allSelected ? true : (noneSelected ? false : null);
        OnPropertyChanged(nameof(IsAllSelectedAppx));
    }
}