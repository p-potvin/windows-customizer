namespace SizeCacheService
{
    internal static class FolderSizeCalculator
    {
        public static long GetDirectorySize(string path)
        {
            try
            {
                var directory = new DirectoryInfo(path);
                if (!directory.Exists) return 0;

                // Enumerate files and sum their lengths.
                // Using EnumerateFiles is more memory-efficient for large directories.
                return directory.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
            }
            catch (Exception ex)
            {
                // Could be UnauthorizedAccessException, PathTooLongException, etc.
                Console.Error.WriteLine($"[{DateTime.Now}] ERROR calculating size for '{path}': {ex.Message}");
                return -1; // Use -1 to indicate an error
            }
        }
    }
}
