@echo off
echo =========================================
echo  Building Windows Customizer Tools...
echo =========================================
echo.

:: Check for dotnet SDK
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: The .NET SDK is not installed or not in your PATH.
    echo Please install the .NET 8 SDK from https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo Found .NET SDK. Starting build...
echo.

:: Build the ViewManager
echo --- Building ViewManager ---
pushd "src\ViewManager"
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: Failed to build ViewManager.
    popd
    pause
    exit /b 1
)
popd
echo ViewManager built successfully.
echo.


:: Build the SizeCacheService
echo --- Building SizeCacheService ---
pushd "src\SizeCacheService"
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: Failed to build SizeCacheService.
    popd
    pause
    exit /b 1
)
popd
echo SizeCacheService built successfully.
echo.


:: Build the FolderSizeColumn
echo --- Building FolderSizeColumn ---
pushd "src\FolderSizeColumn"
dotnet build --configuration Release
if %errorlevel% neq 0 (
    echo ERROR: Failed to build FolderSizeColumn.
    popd
    pause
    exit /b 1
)
popd
echo FolderSizeColumn built successfully.
echo.

echo =========================================
echo  All tools built successfully!
echo =========================================
echo You can now run register_components.bat to install the Explorer column.
echo.
pause
