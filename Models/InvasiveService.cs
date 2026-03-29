using WindowsCustomizer.Common;

namespace WindowsCustomizer.Models;

public class InvasiveService : ObservableObject
{
    private bool _isEnabled;

    public string Name { get; set; }
    public string Description { get; set; }
    
    // True means the invasive feature/service is currently ENABLED in windows (ie we want to uncheck it to turn it off)
    public bool IsEnabled 
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }
}
