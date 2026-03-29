using Xunit;
using WindowsCustomizer.Models;

namespace WindowsCustomizer.Tests;

public class EssentialProgramTests
{
    [Fact]
    public void IsSelected_PropertyChanged_FiresToggle()
    {
        // Arrange
        var prog = new EssentialProgram { Name = "Test", WingetId = "Test.Id" };
        bool propertyChangedFired = false;
        prog.PropertyChanged += (s, e) => {
            if (e.PropertyName == nameof(EssentialProgram.IsSelected))
            {
                propertyChangedFired = true;
            }
        };

        // Act
        prog.IsSelected = true;

        // Assert
        Assert.True(propertyChangedFired);
        Assert.True(prog.IsSelected);
    }

    [Fact]
    public void Properties_SetAndGet()
    {
        var p = new EssentialProgram { Name = "A", Category = "B", Description = "C", WingetId = "D" };
        Assert.Equal("A", p.Name);
        Assert.Equal("B", p.Category);
        Assert.Equal("C", p.Description);
        Assert.Equal("D", p.WingetId);
    }
}
