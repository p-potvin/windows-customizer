using Microsoft.UI.Xaml.Controls;
using WindowsCustomizer.ViewModels;

namespace WindowsCustomizer.Views;

public sealed partial class InvasiveServicesPage : Page
{
    public InvasiveServicesViewModel ViewModel { get; } = new();

    public InvasiveServicesPage()
    {
        this.InitializeComponent();
    }
}
