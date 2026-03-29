using WindowsCustomizer.Common;

namespace WindowsCustomizer.Models;

public class EssentialProgram : ObservableObject
{
    private bool _isSelected;
    public string Name { get; set; }
    public string Description { get; set; }
    public string WingetId { get; set; }
    public string Category { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
