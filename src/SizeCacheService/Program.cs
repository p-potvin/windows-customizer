using System.Text.Json;

namespace SizeCacheService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--------------------------");
            Console.WriteLine("  Size Cache Service v1.0   ");
            Console.WriteLine("--------------------------");
            Console.WriteLine("This service runs in the background to proactively cache folder sizes.");
            Console.WriteLine("Close this window to stop the service.");
            Console.WriteLine();

            var config = LoadConfiguration();
            if (config == null || !config.FoldersToWatch.Any())
            {
                Console.Error.WriteLine("Configuration is missing or contains no folders to watch.");
                Console.Error.WriteLine("Please create a 'config.json' file next to the executable with a 'FoldersToWatch' array of paths.");
                Console.ReadLine();
                return;
            }
            
            var watcherManager = new WatcherManager();
            watcherManager.Start(config.FoldersToWatch);

            var autoEvent = new AutoResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                autoEvent.Set();
            };

            Console.WriteLine("Service is running. Press Ctrl+C to exit.");
            
            // Wait for Ctrl+C
            autoEvent.WaitOne();

            Console.WriteLine("Ctrl+C detected. Shutting down...");
            // Final save before exiting
            CacheManager.SaveCacheToFile();
            Console.WriteLine("Service stopped.");
        }

        private static AppConfig? LoadConfiguration()
        {
            const string configFileName = "config.json";
            if (!File.Exists(configFileName))
            {
                return null;
            }
            var jsonString = File.ReadAllText(configFileName);
            return JsonSerializer.Deserialize<AppConfig>(jsonString);
        }
    }

    internal class AppConfig
    {
        public List<string> FoldersToWatch { get; set; } = new();
    }
}
