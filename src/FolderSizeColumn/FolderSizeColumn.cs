using SharpShell.Attributes;
using SharpShell.SharpColumnHandler;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Collections.Concurrent;

namespace FolderSizeColumn
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFolders)]
    public class FolderSizeColumnHandler : SharpColumnHandler
    {
        // Define a record to match the structure in sizecache.json
        internal record CacheEntry
        {
            public long Size { get; set; }
            public DateTime LastCalculatedUtc { get; set; }
        }

        // The path to the cache file. This creates a dependency on the other project's output location.
        private const string CacheFileName = @"..\SizeCacheService\sizecache.json";
        private static readonly string _cachePath;

        static FolderSizeColumnHandler()
        {
            // Determine the absolute path to the cache file relative to this DLL's location.
            var dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var dllDir = Path.GetDirectoryName(dllPath);
            _cachePath = Path.GetFullPath(Path.Combine(dllDir, CacheFileName));
        }

        /// <summary>
        /// Defines the custom column.
        /// </summary>
        protected override Column GetColumn()
        {
            return new Column("Folder Size", new ColumnDetails
            {
                Title = "Folder Size",
                Width = 100,
                Alignment = ColumnAlignment.Right,
                // This tells Explorer to treat the data as a file size and format it nicely (KB, MB, etc.)
                Format = Win32.Shell.PSFORMAT.PSFORMAT_FILESIZE 
            });
        }

        /// <summary>
        /// Gets the column data for a specific item.
        /// </summary>
        /// <param name="itemPath">The path of the item (in this case, a folder).</param>
        /// <returns>The data to be displayed in the column.</returns>
        protected override object GetData(string itemPath)
        {
            // This handler is for folders only.
            if (!Directory.Exists(itemPath))
                return null;

            try
            {
                if (!File.Exists(_cachePath))
                    return null;

                // We read the file on each call. This is not the most performant way,
                // but it's the simplest and safest from a shell extension.
                // A more advanced version could use a named pipe or memory-mapped file
                // to communicate with the service directly.
                var jsonString = File.ReadAllText(_cachePath);
                var cache = JsonSerializer.Deserialize<ConcurrentDictionary<string, CacheEntry>>(jsonString);

                if (cache != null && cache.TryGetValue(itemPath.ToUpperInvariant(), out var entry))
                {
                    // The data must be returned as a long (Int64) for the PSFORMAT_FILESIZE to work.
                    return (long)entry.Size;
                }
            }
            catch (System.Exception ex)
            {
                // We must be very careful about exceptions in shell extensions.
                // Log the error for debugging but don't crash Explorer.
                File.AppendAllText("c:	emp\FolderSizeColumnError.log", ex.ToString());
                return null;
            }

            return null;
        }
    }
}
