<#
.SYNOPSIS
Calculates the total size of a given folder.

.DESCRIPTION
This script recursively calculates the size of all files within a specified folder
and displays the result in a human-readable format (Bytes, KB, MB, GB, TB).

.PARAMETER Path
The full path to the folder you want to measure.

.EXAMPLE
PS C:\> .\Get-FolderSize.ps1 -Path "C:\Windows"
#>

[CmdletBinding()]
param (
    [Parameter(Mandatory = $true, ValueFromPipeline = $true, Position = 0)]
    [string]$Path
)

process {
    if (-not (Test-Path -Path $Path -PathType Container)) {
        Write-Error "Path does not exist or is not a folder: $Path"
        return
    }

    Write-Host "Calculating size of folder: $Path"
    Write-Host "This may take a while for large folders..."

    try {
        $totalSize = (Get-ChildItem -Path $Path -Recurse -File -Force -ErrorAction Stop | Measure-Object -Property Length -Sum).Sum
    }
    catch {
        Write-Error "An error occurred while calculating the folder size. Check permissions. Error: $_"
        return
    }

    $humanReadableSize = switch ($totalSize) {
        { $_ -ge 1TB } { "{0:N2} TB" -f ($_ / 1TB) }
        { $_ -ge 1GB } { "{0:N2} GB" -f ($_ / 1GB) }
        { $_ -ge 1MB } { "{0:N2} MB" -f ($_ / 1MB) }
        { $_ -ge 1KB } { "{0:N2} KB" -f ($_ / 1KB) }
        default { "{0} Bytes" -f $_ }
    }

    Write-Host ""
    Write-Host "Total Folder Size: $humanReadableSize"
    Write-Host "Total Bytes: $totalSize"
    
    # Keep the window open to show the result
    Write-Host ""
    Read-Host "Press Enter to close..."
}
