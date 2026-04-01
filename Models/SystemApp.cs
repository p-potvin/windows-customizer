using WindowsCustomizer.Common;

namespace WindowsCustomizer.Models
{
    public class SystemApp : ObservableObject
    {
        private bool _isSelected;
        public string Name { get; set; }
        public string Type { get; set; } // e.g., "AppX", "Service", "Startup Task"
        public string InternalName { get; set; } // e.g., PackageFamilyName for AppX

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
