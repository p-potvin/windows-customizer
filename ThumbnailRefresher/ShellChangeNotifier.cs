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

public class ShellChangeNotifier : IDisposable
{
    private readonly ILogger _logger;
    private readonly Action<string> _onFolderChanged;
    private readonly CancellationTokenSource _cts = new();

    public ShellChangeNotifier(ILogger logger, Action<string> onFolderChanged)
    {
        _logger = logger;
        _onFolderChanged = onFolderChanged;
    }

    public void StartMonitoring(CancellationToken stoppingToken)
    {
        // For a standalone service, we use IShellWindows to track active explorer windows
        // and watch for path changes.
        Task.Run(async () =>
        {
            var clsid = Type.GetTypeFromCLSID(new Guid("9BA05972-F6A8-11CF-A442-00A0C90A8F39"));
            if (clsid == null) return;

            var shellWindows = (dynamic)Activator.CreateInstance(clsid)!;
            string lastDetectedPath = string.Empty;

            while (!stoppingToken.IsCancellationRequested && !_cts.Token.IsCancellationRequested)
            {
                try
                {
                    if (shellWindows == null) break;
                    
                    string? topPath = null;
                    
                    // Query for the active Explorer windows. 
                    // Note: This iterates through all open windows.
                    foreach (var window in shellWindows)
                    {
                        try 
                        {
                            if (window?.Document?.Folder?.Self == null) continue;
                            
                            // We take the first one found as the 'current' context for the refresh.
                            // To improve this, one could check for window focus via Win32 API, 
                            // but for now, we'll just ensure we don't spam the same path twice.
                            string path = (string)window.Document.Folder.Self.Path;
                            
                            if (!string.IsNullOrEmpty(path))
                            {
                                topPath = path;
                                break; // Found a valid folder path
                            }
                        }
                        catch { continue; }
                    }

                    if (topPath != null && topPath != lastDetectedPath)
                    {
                        var oldPath = lastDetectedPath;
                        lastDetectedPath = topPath;
                        _logger.LogInformation("Path change detected: {Old} -> {New}", oldPath, lastDetectedPath);
                        _onFolderChanged(lastDetectedPath);
                    }
                    else if (topPath == null)
                    {
                        _logger.LogTrace("No active Explorer windows found with valid folders.");
                    }
                }
                catch (COMException ex) { _logger.LogTrace("COM link to Explorer busy or closed: {Msg}", ex.Message); }
                catch (Exception ex) { _logger.LogError(ex, "Error monitoring shell windows."); }

                await Task.Delay(1000, stoppingToken); // Check for window switches/new windows
            }
        }, stoppingToken);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
