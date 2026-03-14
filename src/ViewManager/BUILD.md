# Building the ViewManager Helper

This project includes a C# helper application, `ViewManager`, that is required for the advanced folder view management features. It must be compiled before use.

## Prerequisites

You need to have the .NET 8 SDK installed on your system. You can download it from the official Microsoft website:

[https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

## Build Instructions

1.  **Open a Command Prompt or PowerShell terminal.**
2.  **Navigate to the project directory:**
    ```sh
    cd c:\path	o\your\windows-customizer\src\ViewManager
    ```
3.  **Run the build command:**
    ```sh
    dotnet build --configuration Release
    ```
This command will compile the application. The output executable will be located in the `bin\Release
et8.0-windows` sub-directory.

The final executable will be named `view-manager.exe`.

## Usage

Once built, the PowerShell scripts in this project will automatically look for this executable. You can also run it directly from the command line to test it:

```sh
# Navigate to the output directory
cd bin\Release
et8.0-windows

# Run the executable with --help to see the available commands
.\view-manager.exe --help
```
