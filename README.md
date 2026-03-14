# Windows Customizer

A project to centralize and manage customizations for Windows Explorer.

# Windows Customizer

Welcome to Windows Customizer! This project gives you a set of tools to supercharge Windows Explorer with features you've always wanted.

## Features

*   **See Folder Sizes Instantly**: Adds a new "Folder Size" column directly in Explorer, which updates automatically for folders you choose to monitor.
*   **Save & Restore Custom Folder Views**: Save the complete layout of a folder (columns, sorting, grouping, etc.) and apply it to any other folder. Perfect for standardizing your project folders!
*   **On-Demand Size Calculation**: Right-click any folder to calculate its size instantly.

---

## Installation Guide

Follow these three steps to get everything set up.

### Prerequisite: Install .NET 8

This application is built on .NET, a free and official developer platform from Microsoft. You only need to install this once.

1.  Go to the official .NET 8 download page: [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
2.  Click the "x64" button under the "SDK" column to download the installer.
3.  Run the installer and complete the installation using the default options.

### Step 1: Build the Tools

Now we need to assemble the helper applications from the source code.

Simply double-click the **`build_all.bat`** file located in the main project folder. A command window will appear and show the progress. This may take a minute or two. Press any key to close the window when it's finished.

### Step 2: Install the "Folder Size" Column

To make the "Folder Size" column available in Explorer, you need to register it with Windows.

1.  Right-click on the **`register_components.bat`** file.
2.  Select **"Run as administrator"**.
3.  Approve the security prompt. A window will appear and confirm the installation.

### Step 3: Start the Caching Service

For the "Folder Size" column to work automatically, a service needs to run in the background.

1.  Navigate to the `src\SizeCacheService\bin\Release` folder.
2.  Double-click the **`size-cache-service.exe`** file.
3.  A command window will appear and stay open. **You can minimize this window, but keep it running** for the service to work.

**Pro Tip**: For convenience, you can have this service start automatically with Windows.
1.  Right-click on `size-cache-service.exe` and select "Create shortcut".
2.  Press `Win + R` to open the Run dialog, type `shell:startup`, and press Enter.
3.  Drag the newly created shortcut into the Startup folder that opens.

---

## How to Use the Features

### Using the "Folder Size" Column

1.  **Restart Explorer**: First, restart Windows Explorer to make sure it sees the new column. Open Task Manager (`Ctrl+Shift+Esc`), find "Windows Explorer", right-click it, and select "Restart".
2.  **Configure Folders**: Open the `src\SizeCacheService\config.json` file in a text editor like Notepad. Add the paths of the main folders you want to monitor (e.g., `C:\\Users\\YourName\\Documents`). Save the file and restart `size-cache-service.exe` if it was already running.
3.  **Enable the Column**: Open one of the folders you are monitoring. Right-click on any column header (like "Name"), click "More...", scroll to find "**Folder Size**", check the box, and click OK. The column will now show the cached sizes!

### Saving and Applying Folder Views

This feature is used from the command line.

1.  Navigate to the `scripts` folder inside the project directory.
2.  In the address bar, type `powershell` and press Enter. This will open a PowerShell window in the correct location.
3.  Use the following commands:

    *   **To save a view:** Set up a folder exactly how you like it. Then, run this command, replacing the path and name:
        ```powershell
        .\Manage-FolderViews.ps1 Save-PersistentFolderView -Path "C:\path\to\your\folder" -Name "MyPerfectView"
        ```

    *   **To apply a view:** To make another folder look the same, run this command:
        ```powershell
        .\Manage-FolderViews.ps1 Set-PersistentFolderView -Path "C:\path\to\another\folder" -Name "MyPerfectView"
        ```
    *   **To see your saved views:**
        ```powershell
        .\Manage-FolderViews.ps1 Get-PersistentFolderView
        ```

### Calculating Folder Size On-Demand

Right-click on any folder and select **"Calculate Folder Size"**. A window will pop up with the result. This uses the cache if available, so it's super fast for monitored folders!

---

## Uninstallation

To remove the components from your system:

1.  Right-click on **`unregister_components.bat`** and select **"Run as administrator"**.
2.  Close the background service (`size-cache-service.exe`).
3.  Remove the shortcut from the Startup folder if you added it.
