using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThumbnailRefresher.Native;

namespace ThumbnailRefresher;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly uint _requestedSize = 96;
    private ShellChangeNotifier? _notifier;

    // Cancellation support for folder switches
    private CancellationTokenSource? _currentTaskCts;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ThumbnailRefresher Service started.");

        _notifier = new ShellChangeNotifier(_logger, (newFolderPath) =>
        {
            // Folder changed: cancel existing and start new
            CancelActiveTask();
            _currentTaskCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            _ = ProcessNewFolderAsync(newFolderPath, _currentTaskCts.Token);
        });

        _notifier.StartMonitoring(stoppingToken);

        // Keep the service alive until stopped
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        
        CancelActiveTask();
    }

    private void CancelActiveTask()
    {
        try
        {
            _currentTaskCts?.Cancel();
        }
        catch (ObjectDisposedException) { }
        finally
        {
            _currentTaskCts?.Dispose();
            _currentTaskCts = null;
        }
    }

    private async Task ProcessNewFolderAsync(string folderPath, CancellationToken ct)
    {
        _logger.LogTrace("Awaiting debounce for: {Path}", folderPath);
        // 0.2s Delay as requested to cover fast browsing
        await Task.Delay(200, ct);
        if (ct.IsCancellationRequested)
        {
            _logger.LogTrace("Debounce cancelled: User moved on from {Path}", folderPath);
            return;
        }

        _logger.LogInformation("Processing folder: {Path}", folderPath);

        string[] files;
        try
        {
            files = Directory.GetFiles(folderPath);
            _logger.LogInformation("Found {Count} candidate files in {Path}", files.Length, folderPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read directory: {Path}", folderPath);
            return;
        }

        int processed = 0;
        int skipped = 0;
        foreach (var file in files)
        {
            if (ct.IsCancellationRequested)
            {
                _logger.LogInformation("Cancelled processing {Path} (Processed: {Proc}/{Total})", folderPath, processed, files.Length);
                return;
            }

            try
            {
                if (RefreshThumbnail(file)) processed++;
                else skipped++;

                // Batching: 10 items at a time
                if (processed % 10 == 0 && processed > 0)
                {
                    _logger.LogTrace("Batch checkpoint ({Proc}/{Total})", processed, files.Length);
                    await Task.Delay(50, ct); // Short yield delay
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Unexpected error processing {File}: {Msg}", Path.GetFileName(file), ex.Message);
            }
        }

        _logger.LogInformation("Completed: {Path} (Extracted: {Proc}, Skipped: {Skip})", folderPath, processed, skipped);
    }

    private bool RefreshThumbnail(string filePath)
    {
        try
        {
            // Filter by extension - only process common image/video types to avoid fighting with system for odd files
            string ext = Path.GetExtension(filePath).ToLower();
            string[] supportedExts = { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".gif", ".mp4", ".mkv", ".mov" };
            if (Array.IndexOf(supportedExts, ext) == -1)
            {
                _logger.LogTrace("Skipping unsupported extension: {File}", Path.GetFileName(filePath));
                return false;
            }

            _logger.LogTrace("Processing: {File}", Path.GetFileName(filePath));

            Guid iidShellItem = ShellNativeMethods.IID_IShellItem;
            ShellNativeMethods.SHCreateItemFromParsingName(filePath, IntPtr.Zero, ref iidShellItem, out IShellItem shellItem);
            
            // Check for dehydration / offline status
            uint attributes;
            // Get offline and storage status
            shellItem.GetAttributes(ShellNativeMethods.SFGAO_OFFLINE | ShellNativeMethods.SFGAO_STORAGE | ShellNativeMethods.SFGAO_STREAM, out attributes);
            bool isOffline = (attributes & ShellNativeMethods.SFGAO_OFFLINE) != 0;
            bool isFolder = (attributes & ShellNativeMethods.SFGAO_STORAGE) != 0 && (attributes & ShellNativeMethods.SFGAO_STREAM) == 0;

            if (isFolder)
            {
                _logger.LogTrace("Skipping subfolder: {File}", Path.GetFileName(filePath));
                return false;
            }

            // Request 256px thumbnail (Standard Large)
            // WTS_EXTRACT: Extract if not in cache
            // WTS_FORCEEXTRACTION: Re-extract even if in cache
            // WTS_SCALETOREQUESTEDSIZE: Ensure it matches target size
            // WTS_SLOWRECLAIM: Needed for complex extractions (videos/large images)
            WTS_FLAGS flags = WTS_FLAGS.WTS_EXTRACT | WTS_FLAGS.WTS_FORCEEXTRACTION | WTS_FLAGS.WTS_SCALETOREQUESTEDSIZE | WTS_FLAGS.WTS_SLOWRECLAIM;

            if (isOffline)
            {
                // For cloud items, we actually WANT to trigger the download if it's small or metadata is available
                // We'll remove WTS_EXTRACTDONOTRETRIEVE to allow the shell to try harder
                _logger.LogTrace("Cloud file detected: {Path}", filePath);
            }

            // Bind to the thumbnail cache for this specific item
            Guid bhidThumbnailHandler = ShellNativeMethods.BHID_ThumbnailHandler;
            Guid iidThumbnailCache = ShellNativeMethods.IID_IThumbnailCache;
            
            shellItem.BindToHandler(IntPtr.Zero, ref bhidThumbnailHandler, ref iidThumbnailCache, out IntPtr pCachePtr);
            
            if (pCachePtr != IntPtr.Zero)
            {
                IThumbnailCache? itemCache = Marshal.GetObjectForIUnknown(pCachePtr) as IThumbnailCache;
                try
                {
                    if (itemCache != null)
                    {
                        // Request thumbnail with specified size and flags
                        // Using 256 as it forces a "real" high-res extraction that Explorer actually notices
                        itemCache.GetThumbnail(shellItem, 256, flags, out ISharedBitmap sharedBitmap, out WTS_CACHEFLAGS cacheFlags, out _);
                        
                        // Access the bitmap to ensure the COM call actually finishes the work
                        if (sharedBitmap != null)
                        {
                            sharedBitmap.GetBitmap(out IntPtr hbm);
                            if (hbm != IntPtr.Zero)
                            {
                                ShellNativeMethods.DeleteObject(hbm);
                            }
                            Marshal.ReleaseComObject(sharedBitmap);
                            _logger.LogTrace("Extracted: {File} (Cache: {Cache})", Path.GetFileName(filePath), cacheFlags);
                            return true;
                        }
                    }
                }
                finally
                {
                    if (pCachePtr != IntPtr.Zero) Marshal.Release(pCachePtr);
                }
            }
            
            _logger.LogTrace("Failed to bind thumbnail handler for: {File}", Path.GetFileName(filePath));
        }
        catch (COMException ex) { 
            _logger.LogTrace("COM Error for {File}: 0x{HResult:X}", Path.GetFileName(filePath), ex.HResult);
        }
        catch (Exception ex)
        {
            _logger.LogTrace("Extraction error for {File}: {Msg}", Path.GetFileName(filePath), ex.Message);
        }
        return false;
    }

    public override void Dispose()
    {
        _notifier?.Dispose();
        CancelActiveTask();
        base.Dispose();
    }
}
