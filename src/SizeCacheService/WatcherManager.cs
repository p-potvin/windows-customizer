using System.Collections.Concurrent;

namespace SizeCacheService
{
    internal class WatcherManager
    {
        private readonly List<FileSystemWatcher> _watchers = new();
        private readonly ConcurrentDictionary<string, byte> _recalculationQueue = new();
        private Timer? _debounceTimer;
        private readonly TimeSpan _debouncePeriod = TimeSpan.FromSeconds(10);
        private readonly object _lock = new();

        public void Start(List<string> rootFolders)
        {
            Console.WriteLine($"[{DateTime.Now}] Starting to watch {rootFolders.Count} root folder(s).");

            foreach (var rootFolder in rootFolders)
            {
                if (Directory.Exists(rootFolder))
                {
                    var allDirs = Directory.GetDirectories(rootFolder, "*", SearchOption.AllDirectories).ToList();
                    allDirs.Add(rootFolder);

                    foreach (var dir in allDirs)
                    {
                        var watcher = new FileSystemWatcher(dir)
                        {
                            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size
                        };
                        watcher.Created += OnChange;
                        watcher.Deleted += OnChange;
                        watcher.Changed += OnChange;
                        watcher.Renamed += OnRenamed;
                        watcher.EnableRaisingEvents = true;
                        _watchers.Add(watcher);
                    }
                }
                else
                {
                    Console.Error.WriteLine($"[{DateTime.Now}] WARNING: Path not found, cannot watch: {rootFolder}");
                }
            }
            Console.WriteLine($"[{DateTime.Now}] Watching {_watchers.Count} directories.");
        }

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            // The change happens in a directory, so we need its path.
            var dirPath = Path.GetDirectoryName(e.FullPath);
            if (dirPath != null)
            {
                // Also trigger a recalc for the parent directory
                var parentDir = Directory.GetParent(dirPath);
                if(parentDir != null)
                {
                    QueueRecalculation(parentDir.FullName);
                }
                QueueRecalculation(dirPath);
            }
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            var oldDirPath = Path.GetDirectoryName(e.OldFullPath);
            if (oldDirPath != null)
            {
                 QueueRecalculation(oldDirPath);
            }

            var newDirPath = Path.GetDirectoryName(e.FullPath);
             if (newDirPath != null)
            {
                 QueueRecalculation(newDirPath);
            }
        }

        private void QueueRecalculation(string path)
        {
            Console.WriteLine($"[{DateTime.Now}] Change detected in: {path}. Queuing for recalculation.");
            _recalculationQueue.TryAdd(path, 0);

            lock (_lock)
            {
                // If timer is not running, start it.
                _debounceTimer ??= new Timer(ProcessQueue, null, _debouncePeriod, Timeout.InfiniteTimeSpan);
            }
        }

        private void ProcessQueue(object? state)
        {
            var pathsToProcess = _recalculationQueue.Keys.ToList();
            _recalculationQueue.Clear();

            if (!pathsToProcess.Any())
            {
                lock (_lock)
                {
                    _debounceTimer?.Dispose();
                    _debounceTimer = null;
                }
                return;
            }
            
            Console.WriteLine($"[{DateTime.Now}] Debounce timer fired. Processing {pathsToProcess.Count} updated directories.");
            foreach (var path in pathsToProcess)
            {
                var size = FolderSizeCalculator.GetDirectorySize(path);
                if (size >= 0)
                {
                    var entry = new CacheEntry { Size = size, LastCalculatedUtc = DateTime.UtcNow };
                    CacheManager.Set(path, entry);
                    Console.WriteLine($"[{DateTime.Now}] Updated cache for '{path}': {size} bytes.");
                }
            }
            
            CacheManager.SaveCacheToFile();
            
            lock(_lock)
            {
                // Reset the timer for the next debounce period
                _debounceTimer?.Change(_debouncePeriod, Timeout.InfiniteTimeSpan);
            }
        }
    }
}
