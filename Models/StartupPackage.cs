using WindowsCustomizer.Common;

namespace WindowsCustomizer.Models;

public class StartupPackage : ObservableObject
{
    private bool _isSelected;
    public string Name { get; set; }
    public string Command { get; set; }
    public string User { get; set; }
    public string Location { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
