@echo off

:: Check for Administrator privileges
openfiles >nul 2>&1
if %errorlevel% neq 0 (
    echo ====================================================================
    echo  ERROR: This script must be run as an Administrator.
    echo ====================================================================
    echo.
    echo  Please right-click on this file (unregister_components.bat) and
    echo  select "Run as administrator".
    echo.
    pause
    exit /b 1
)

echo ===================================================
echo  Un-registering Windows Explorer Components...
echo ===================================================
echo.

set DLL_PATH="src\FolderSizeColumn\bin\Release\FolderSizeColumn.comhost.dll"

if not exist %DLL_PATH% (
    echo WARNING: The main component %DLL_PATH% was not found.
    echo Skipping un-registration.
    pause
    exit /b 0
)

echo Un-registering the Folder Size column...
regsvr32.exe /u /s %DLL_PATH%

if %errorlevel% neq 0 (
    echo ERROR: Un-registration failed.
    pause
    exit /b 1
)

echo.
echo ====================================================================
echo  Component un-registered successfully!
echo ====================================================================
echo.
echo  Please restart Windows Explorer to complete the process.
echo.
pause
