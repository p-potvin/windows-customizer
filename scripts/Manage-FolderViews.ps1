<#
.SYNOPSIS
A script to manage Windows Explorer folder view settings, both globally and for specific folders.

.DESCRIPTION
This script provides functions to back up and restore all folder views globally.
It also integrates with the 'view-manager.exe' helper application to save, apply,
and manage persistent view settings for individual folders.

.NOTES
Author: Gemini
Date: 14/03/2026
#>

# --- Configuration ---
# The script expects the compiled helper executable to be in this location.
# Run the build as described in src/ViewManager/BUILD.md
$viewManagerPath = Join-Path $PSScriptRoot "..\src\ViewManager\bin\Release\net8.0-windows\view-manager.exe"

# --- Per-Folder View Management (Requires view-manager.exe) ---

function Save-PersistentFolderView {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path,
        [Parameter(Mandatory=$true)]
        [string]$Name
    )
    
    if (-not (Test-Path $viewManagerPath)) {
        Write-Error "View Manager executable not found at '$viewManagerPath'. Please build it first."
        return
    }

    Write-Host "Saving view for '$Path' as '$Name'..."
    & $viewManagerPath save --path $Path --name $Name
}

function Set-PersistentFolderView {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Path,
        [Parameter(Mandatory=$true)]
        [string]$Name
    )

    if (-not (Test-Path $viewManagerPath)) {
        Write-Error "View Manager executable not found at '$viewManagerPath'. Please build it first."
        return
    }

    Write-Host "Applying view '$Name' to '$Path'..."
    & $viewManagerPath apply --path $Path --name $Name
}

function Get-PersistentFolderView {
    [CmdletBinding()]
    param()

    if (-not (Test-Path $viewManagerPath)) {
        Write-Error "View Manager executable not found at '$viewManagerPath'. Please build it first."
        return
    }

    & $viewManagerPath list
}

function Remove-PersistentFolderView {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$Name
    )

    if (-not (Test-Path $viewManagerPath)) {
        Write-Error "View Manager executable not found at '$viewManagerPath'. Please build it first."
        return
    }
    
    Write-Host "Deleting view '$Name'..."
    & $viewManagerPath delete --name $Name
}


# --- Global View Management ---

function Backup-AllFolderViews {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$BackupPath
    )

    if (-not (Test-Path $BackupPath)) {
        New-Item -Path $BackupPath -ItemType Directory | Out-Null
        Write-Host "Created backup directory: $BackupPath"
    }

    Write-Host "Saving all folder view settings to $BackupPath..."

    $regKeysToSave = @(
        "HKCU\Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\BagMRU",
        "HKCU\Software\classes\Local Settings\Software\Microsoft\Windows\Shell\Bags",
        "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\FolderTypes",
        "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Streams\Defaults"
    )

    foreach ($key in $regKeysToSave) {
        $fileName = ($key -replace ':', '_') -replace '\\', '_'
        $filePath = Join-Path $BackupPath "$fileName.reg"
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

function Restore-AllFolderViews {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$BackupPath
    )

    if (-not (Test-Path $BackupPath)) {
        Write-Error "Backup path '$BackupPath' does not exist."
        return
    }

    $regFiles = Get-ChildItem -Path $BackupPath -Filter "*.reg"
    if (-not $regFiles) {
        Write-Warning "No .reg files found in $BackupPath to restore."
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

    Write-Host "Importing registry files from $BackupPath..."
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

function Reset-AllFolderViews {
    [CmdletBinding()]
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
