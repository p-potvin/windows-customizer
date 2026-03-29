using WindowsCustomizer.Common;

namespace WindowsCustomizer.Models;

public class ExplorerSetting : ObservableObject
{
    private bool _isEnabled;
    public string Name { get; set; }
    public string Description { get; set; }
    public string RegistryKey { get; set; }
    public string RegistryValue { get; set; }
    public object EnabledValue { get; set; }
    public object DisabledValue { get; set; }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }
}
