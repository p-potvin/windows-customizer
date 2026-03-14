# Windows Customizer

A project to centralize and manage customizations for Windows Explorer.

## Features

This tool aims to provide a centralized interface for the following customizations:

- **Specific Folder Views:** Assign and lock specific view settings (e.g., Details, Large Icons) to particular folders.
- **Custom Columns:** Add new data columns to the Details view in Explorer.
- **Folder Size Calculation:** Enable folder size calculations to be displayed in a column.
- **Custom Tooltips:** Modify the information displayed in tooltips for files and folders.

## Feature: Calculate Folder Size (via Context Menu)

This feature adds a "Calculate Folder Size" option to the right-click context menu for folders in Windows Explorer.

### How it Works

The context menu option runs a PowerShell script (`Get-FolderSize.ps1`) that recursively scans the selected folder, calculates the total size of all files within it, and displays the result in a command window.

### Installation

1.  **Edit the Registry File**: Open the `scripts\Add-FolderSize-ContextMenu.reg` file in a text editor.
2.  **Update the Path**: You MUST replace the placeholder path (`C:\\path\\to\\your\\scripts\\Get-FolderSize.ps1`) with the absolute path to the `Get-FolderSize.ps1` script on your machine. Remember to use double backslashes (`\\`) for the path.
3.  **Run the Registry File**: Double-click the `Add-FolderSize-ContextMenu.reg` file to merge it into the Windows Registry. You will need to approve the security prompts.

Once installed, you can right-click on any folder to see the new option.

## Disclaimer

This project modifies the Windows Registry and interacts with system-level components. Use it at your own risk. Always back up your registry before applying changes.
