using System.Text.Json;
using System.Collections.Concurrent;

namespace SizeCacheService
{
    internal record CacheEntry
    {
        public long Size { get; set; }
        public DateTime LastCalculatedUtc { get; set; }
    }

    internal static class CacheManager
    {
        private const string CacheFileName = "sizecache.json";
        private static readonly ConcurrentDictionary<string, CacheEntry> _cache;
        private static readonly object _fileLock = new();

        static CacheManager()
        {
            _cache = LoadCacheFromFile();
        }

        public static CacheEntry? Get(string path)
        {
            _cache.TryGetValue(path.ToUpperInvariant(), out var entry);
            return entry;
        }

        public static void Set(string path, CacheEntry entry)
        {
            _cache[path.ToUpperInvariant()] = entry;
        }

        public static void SaveCacheToFile()
        {
            lock (_fileLock)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(_cache, options);
                File.WriteAllText(CacheFileName, jsonString);
            }
            Console.WriteLine($"[{DateTime.Now}] Cache saved to disk.");
        }

        private static ConcurrentDictionary<string, CacheEntry> LoadCacheFromFile()
        {
            lock (_fileLock)
            {
                if (!File.Exists(CacheFileName))
                {
                    return new ConcurrentDictionary<string, CacheEntry>();
                }
                var jsonString = File.ReadAllText(CacheFileName);
                return JsonSerializer.Deserialize<ConcurrentDictionary<string, CacheEntry>>(jsonString) ?? new ConcurrentDictionary<string, CacheEntry>();
            }
        }
    }
}
