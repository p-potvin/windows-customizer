using System.ComponentModel;

namespace WindowsCustomizer.Models
{
    public class SystemApp : INotifyPropertyChanged
    {
        private bool _isSelected;
        public string Name { get; set; }
        public string Type { get; set; } // e.g., "AppX", "Service", "Startup Task"
        public string InternalName { get; set; } // e.g., PackageFamilyName for AppX

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
