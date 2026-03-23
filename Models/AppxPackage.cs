using WindowsCustomizer.Common;

namespace WindowsCustomizer.Models;

public class AppxPackage : ObservableObject
{
    private bool _isSelected;
    public string Name { get; set; }
    public string PackageFullName { get; set; }
    public string Publisher { get; set; }
    public string Version { get; set; }
    public string InstallLocation { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}