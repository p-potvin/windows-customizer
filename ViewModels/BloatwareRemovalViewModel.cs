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

    public bool? IsAllSelected
    {
        get => _isAllSelected;
        set
        {
            if (SetProperty(ref _isAllSelected, value) && _isAllSelected.HasValue)
            {
                SelectAll(_isAllSelected.Value);
            }
        }
    }

    public ICommand UninstallSelectedCommand { get; }
    public ObservableCollection<AppxPackage> AppxPackages { get; } = new();

    public BloatwareRemovalViewModel()
    {
        UninstallSelectedCommand = new AsyncRelayCommand(UninstallSelectedAsync, CanUninstall);
    }

    public async Task LoadPackagesAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        AppxPackages.Clear();

        var packages = await PowerShellService.GetAppxPackagesAsync();

        packages.Sort((x, y) => string.Compare(x.Name, y.Name, System.StringComparison.Ordinal));

        foreach (var package in packages)
        {
            package.PropertyChanged += OnPackageSelectionChanged;
            AppxPackages.Add(package);
        }

        IsLoading = false;
        UpdateIsAllSelectedState();
        (UninstallSelectedCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
    }

    private bool CanUninstall()
    {
        return AppxPackages.Any(p => p.IsSelected);
    }

    private async Task UninstallSelectedAsync()
    {
        var selectedPackages = AppxPackages.Where(p => p.IsSelected).ToList();
        if (!selectedPackages.Any()) return;

        IsLoading = true; // Reuse IsLoading to show activity and disable the list

        foreach (var package in selectedPackages)
        {
            await PowerShellService.RemoveAppxPackageAsync(package.PackageFullName);
        }

        await LoadPackagesAsync(); // This will set IsLoading to false
    }

    private void SelectAll(bool select)
    {
        foreach (var package in AppxPackages)
        {
            package.IsSelected = select;
        }
    }

    private void OnPackageSelectionChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppxPackage.IsSelected))
        {
            (UninstallSelectedCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            UpdateIsAllSelectedState();
        }
    }

    private void UpdateIsAllSelectedState()
    {
        bool allSelected = AppxPackages.Any() && AppxPackages.All(p => p.IsSelected);
        bool noneSelected = !AppxPackages.Any() || AppxPackages.All(p => !p.IsSelected);

        // Use a backing field to prevent re-entrancy from the setter
        _isAllSelected = allSelected ? true : (noneSelected ? false : null);
        OnPropertyChanged(nameof(IsAllSelected));
    }
}