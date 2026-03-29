using Xunit;
using WindowsCustomizer.ViewModels;
using System.Linq;

namespace WindowsCustomizer.Tests;

public class EssentialsViewModelTests
{
    [Fact]
    public void LoadPrograms_PopulatesList()
    {
        var vm = new EssentialsViewModel();
        Assert.NotEmpty(vm.Programs);
        Assert.Contains(vm.Programs, p => p.Name == "7-Zip");
    }

    [Fact]
    public void CanInstall_FalseByDefault()
    {
        var vm = new EssentialsViewModel();
        Assert.False(vm.InstallSelectedCommand.CanExecute(null));
    }

    [Fact]
    public void CanInstall_TrueWhenItemSelected()
    {
        var vm = new EssentialsViewModel();
        vm.Programs.First().IsSelected = true;
        Assert.True(vm.InstallSelectedCommand.CanExecute(null));
    }
}
