# Windows Customizer

Windows Customizer is a powerful, WinUI 3-based utility designed for streamlining and customizing Windows 10 and 11 installations. It provides a centralized interface for debloating, installing essential software, and applying deep system tweaks.

## Features

*   **Bloatware Removal**: Identify and uninstall pre-installed Windows applications, disable non-essential services, and manage startup programs.
*   **Essential Program Installation**: Batch install a curated list of third-party programs using the native Windows Package Manager (`winget`).
*   **Explorer Customization**: Tweak Windows Explorer behavior, including toggling hidden files, file extensions, and customizing file/folder icons and info-tips.
*   **Invasive Service Management**: Disable telemetry, OneDrive integration, and other invasive features via registry hooks and firewall configurations.
*   **System Tools**: Easy access to custom `.msc` views and a Microsoft Defender/Firewall killswitch for temporary troubleshooting.
*   **Search Customization**: Disable integrated web search results in the Windows Search bar.
*   **Windhawk Integration**: Support for advanced UI modifications via Windhawk.

## Project Structure

*   **/Common**: Core utilities including `PowerShellService`, `ObservableObject`, and RelayCommand implementations.
*   **/Models**: Data models for system components (AppX packages, Services, Startup items, etc.).
*   **/ViewModels**: MVVM logic for each module/page.
*   **/Views**: WinUI 3 XAML pages for the user interface.
*   **/Windhawk**: Source files for UI customization mods.
*   **/WindowsCustomizer.Tests**: xUnit test project for core logic.

## Prerequisites

*   **OS**: Windows 10 (18362+) or Windows 11.
*   **Developer Environment**:
    *   [Visual Studio 2022](https://visualstudio.microsoft.com/) with:
        *   .NET Desktop Development workload.
        *   Windows Application Development workload (for WinUI 3).
    *   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
*   **Permissions**: Administrator privileges are required to apply most system changes.

## Build and Run

### 1. Clone the Repository
```bash
git clone https://github.com/p-potvin/windows-customizer.git
cd windows-customizer
```

### 2. Restore Dependencies
Open the solution in Visual Studio and it should restore automatically, or run:
```powershell
dotnet restore
```

### 3. Build the Solution
```powershell
dotnet build WindowsCustomizer.csproj -r win-x64
```

### 4. Run the Application
From the CLI:
```powershell
dotnet run --project WindowsCustomizer.csproj -r win-x64 --no-build
```

You can also run the project directly from Visual Studio by selecting `x64` as the platform.

## Testing
The project uses xUnit for unit testing. To run tests:
```powershell
dotnet test
```

## Contributing
Please refer to the [PROJECT_ROADMAP.md](PROJECT_ROADMAP.md) for current development status and [AGENT_INSTRUCTIONS.md](AGENT_INSTRUCTIONS.md) for coding standards.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
