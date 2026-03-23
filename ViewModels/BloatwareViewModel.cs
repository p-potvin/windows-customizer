using System.Collections.ObjectModel;
using System.Windows.Input;
using WindowsCustomizer.Common;
using WindowsCustomizer.Models;

namespace WindowsCustomizer.ViewModels
{
    public class BloatwareViewModel : ViewModelBase
    {
        public ObservableCollection<SystemApp> Items { get; }

        public ICommand LoadItemsCommand { get; }
        public ICommand RemoveSelectedItemsCommand { get; }

        public BloatwareViewModel()
        {
            Items = new ObservableCollection<SystemApp>();
            LoadItemsCommand = new RelayCommand(LoadItems);
            RemoveSelectedItemsCommand = new RelayCommand(RemoveSelectedItems);
        }

        private void LoadItems(object parameter)
        {
            // This is where the logic to get AppX packages, services, etc. will go.
            // For now, I'll add some dummy data.
            Items.Clear();
            Items.Add(new SystemApp { Name = "Candy Crush Saga", Type = "AppX", InternalName = "king.com.CandyCrushSaga", IsSelected = false });
            Items.Add(new SystemApp { Name = "Xbox Game Bar", Type = "AppX", InternalName = "Microsoft.XboxGamingOverlay", IsSelected = false });
            Items.Add(new SystemApp { Name = "Print Spooler", Type = "Service", InternalName = "Spooler", IsSelected = false });
            Items.Add(new SystemApp { Name = "Windows Update", Type = "Service", InternalName = "wuauserv", IsSelected = true });
        }

        private void RemoveSelectedItems(object parameter)
        {
            // This is where the logic to remove the selected items will go.
            // It will iterate through `Items` and remove the ones where `IsSelected` is true.
        }
    }
}
