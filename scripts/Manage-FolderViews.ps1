<#
.SYNOPSIS
A script to manage Windows Explorer folder view settings.

.DESCRIPTION
This script provides functions to save, restore, and reset folder view preferences
stored in the Windows Registry.

.NOTES
Author: Gemini
Date: 14/03/2026

Functions:
- Save-FolderViews
- Restore-FolderViews
- Reset-FolderViews
#>

function Save-FolderViews {
    <#
    .SYNOPSIS
    Saves the current user's folder view settings to a specified directory.
    .DESCRIPTION
    Exports multiple registry keys related to folder views to .reg files.
    .PARAMETER Path
    The directory path where the backup .reg files will be saved.
    .EXAMPLE
    Save-FolderViews -Path "C:\Backups\FolderViewSettings"
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path
    )

    if (-not (Test-Path $Path)) {
        New-Item -Path $Path -ItemType Directory | Out-Null
        Write-Host "Created backup directory: $Path"
    }

    Write-Host "Saving folder view settings to $Path..."

    $regKeysToSave = @(
        "HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU",
        "HKCU\Software\classes\Local Settings\Software\Microsoft\Windows\Shell\Bags",
        "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\FolderTypes",
        "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Streams\Defaults"
    )

    foreach ($key in $regKeysToSave) {
        $fileName = ($key -replace ':', '_') -replace '\\', '_'
        $filePath = Join-Path $Path "$fileName.reg"
        $regPathForExe = $key.Replace("HKCU\", "HKEY_CURRENT_USER\")
        
        Write-Host "Exporting registry key: $regPathForExe"
        try {
            Start-Process "reg" -ArgumentList "export `"$regPathForExe`" `"$filePath`" /y" -Wait -NoNewWindow -ErrorAction Stop
            Write-Host "Successfully exported '$key' to '$filePath'."
        }
        catch {
            Write-Error "Failed to export registry key: $key. Error: $_"
        }
    }
}

function Restore-FolderViews {
    <#
    .SYNOPSIS
    Restores folder view settings from a specified directory.
    .DESCRIPTION
    Imports all .reg files from a backup directory and restarts Explorer.
    .PARAMETER Path
    The directory path containing the .reg backup files.
    .EXAMPLE
    Restore-FolderViews -Path "C:\Backups\FolderViewSettings"
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path
    )

    if (-not (Test-Path $Path)) {
        Write-Error "Backup path '$Path' does not exist."
        return
    }

    $regFiles = Get-ChildItem -Path $Path -Filter "*.reg"
    if (-not $regFiles) {
        Write-Warning "No .reg files found in $Path to restore."
        return
    }

    Write-Host "This will overwrite current folder view settings and restart Explorer."
    $confirmation = Read-Host "Are you sure you want to continue? (y/n)"
    if ($confirmation -ne 'y') {
        Write-Host "Operation cancelled."
        return
    }

    Write-Host "Stopping Explorer process..."
    Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue

    Write-Host "Importing registry files from $Path..."
    foreach ($file in $regFiles) {
        Write-Host "Importing $($file.Name)..."
        try {
            Start-Process "reg" -ArgumentList "import `"$($file.FullName)`"" -Wait -NoNewWindow -ErrorAction Stop
            Write-Host "Successfully imported $($file.Name)."
        }
        catch {
            Write-Warning "Failed to import $($file.Name). Error: $_"
        }
    }

    Write-Host "Restarting Explorer process..."
    Start-Process explorer

    Write-Host "Folder view settings have been restored."
}

function Reset-FolderViews {
    <#
    .SYNOPSIS
    Resets all folder views to Windows defaults.
    .DESCRIPTION
    This function will stop the Explorer process, delete the registry keys
    responsible for storing folder views, and then restart Explorer. This
    action is irreversible unless a backup has been made.
    .EXAMPLE
    Reset-FolderViews
    #>
    Write-Host "This will reset all your folder views to the Windows default."
    $confirmation = Read-Host "Are you sure you want to continue? (y/n)"

    if ($confirmation -ne 'y') {
        Write-Host "Operation cancelled."
        return
    }

    $regKeys = @(
        "HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU",
        "HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags",
        "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\FolderTypes",
        "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Streams\Defaults"
    )

    Write-Host "Stopping Explorer process..."
    Stop-Process -Name explorer -Force

    Write-Host "Deleting folder view registry keys..."
    foreach ($key in $regKeys) {
        $regPath = "Registry::$key"
        if (Test-Path -Path $regPath) {
            try {
                Remove-Item -Path $regPath -Recurse -Force -ErrorAction Stop
                Write-Host "Removed key: $key"
            }
            catch {
                Write-Warning "Could not remove key: $key. It might not exist or there's a permission issue."
            }
        }
    }

    Write-Host "Restarting Explorer process..."
    Start-Process explorer

    Write-Host "Folder views have been reset."
}

# --- Script Entry Point ---
# You can call the functions directly here for testing.
# Example:
#
# Save-FolderViews -Path "$([Environment]::GetFolderPath('Desktop'))\MyFolderViews"
#
# Restore-FolderViews -Path "$([Environment]::GetFolderPath('Desktop'))\MyFolderViews"
#
# Reset-FolderViews
