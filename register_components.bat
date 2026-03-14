@echo off

:: Check for Administrator privileges
openfiles >nul 2>&1
if %errorlevel% neq 0 (
    echo ====================================================================
    echo  ERROR: This script must be run as an Administrator.
    echo ====================================================================
    echo.
    echo  Please right-click on this file (register_components.bat) and
    echo  select "Run as administrator".
    echo.
    pause
    exit /b 1
)

echo =================================================
echo  Registering Windows Explorer Components...
echo =================================================
echo.

set DLL_PATH="src\FolderSizeColumn\bin\Release\FolderSizeColumn.comhost.dll"

if not exist %DLL_PATH% (
    echo ERROR: The main component %DLL_PATH% was not found.
    echo Please run build_all.bat first to create it.
    pause
    exit /b 1
)

echo Registering the Folder Size column...
regsvr32.exe /s %DLL_PATH%

if %errorlevel% neq 0 (
    echo ERROR: Registration failed. Make sure all files are in the correct place.
    pause
    exit /b 1
)

echo.
echo ====================================================================
echo  Component registered successfully!
echo ====================================================================
echo.
echo  To see the changes, you may need to restart Windows Explorer.
echo  You can do this by opening Task Manager, finding "Windows Explorer",
echo  right-clicking it, and choosing "Restart".
echo.
echo  After restarting, you can right-click on any column header in
echo  Explorer (like "Name" or "Date"), click "More...", and check the
echo  box for "Folder Size".
echo.
pause
