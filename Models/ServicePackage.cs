using WindowsCustomizer.Common;

namespace WindowsCustomizer.Models;

public class ServicePackage : ObservableObject
{
    private bool _isSelected;
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Status { get; set; }
    public string StartType { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
