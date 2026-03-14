using System.Text.Json;
using System.IO;

namespace ViewManager
{
    internal record ViewStoreEntry
    {
        public string Name { get; init; }
        public string FileName { get; init; }
        public DateTime Created { get; init; }
    }

    internal static class ViewStore
    {
        private const string StoreFileName = "views.json";
        private const string ViewsDir = "views";

        static ViewStore()
        {
            Directory.CreateDirectory(ViewsDir);
        }

        public static void SaveView(string viewName, Dictionary<string, object> bagData)
        {
            var views = LoadViewStore();
            if (views.Any(v => v.Name.Equals(viewName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"A view with the name '{viewName}' already exists.");
            }

            var dataFileName = $"{Guid.NewGuid()}.json";
            var dataFilePath = Path.Combine(ViewsDir, dataFileName);

            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(bagData, options);
            File.WriteAllText(dataFilePath, jsonString);

            var newEntry = new ViewStoreEntry
            {
                Name = viewName,
                FileName = dataFileName,
                Created = DateTime.UtcNow
            };
            views.Add(newEntry);
            
            SaveViewStore(views);
            Console.WriteLine($"Successfully saved view '{viewName}'.");
        }

        public static Dictionary<string, object>? LoadViewData(string viewName)
        {
            var views = LoadViewStore();
            var entry = views.FirstOrDefault(v => v.Name.Equals(viewName, StringComparison.OrdinalIgnoreCase));

            if (entry == null)
            {
                Console.Error.WriteLine($"No view found with the name '{viewName}'.");
                return null;
            }

            var dataFilePath = Path.Combine(ViewsDir, entry.FileName);
            if (!File.Exists(dataFilePath))
            {
                Console.Error.WriteLine($"Could not find data file '{entry.FileName}' for view '{viewName}'.");
                return null;
            }

            var jsonString = File.ReadAllText(dataFilePath);
            var bagData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
            
            // JSON deserializes byte arrays as JsonElement of Base64 strings. We need to convert them back.
            if (bagData != null)
            {
                var correctedData = new Dictionary<string, object>();
                foreach (var kvp in bagData)
                {
                    if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.String)
                    {
                        try
                        {
                            // Attempt to decode base64, if it fails, it's just a regular string.
                            correctedData[kvp.Key] = Convert.FromBase64String(element.GetString());
                        }
                        catch (FormatException)
                        {
                            correctedData[kvp.Key] = element.GetString();
                        }
                    }
                    else if (kvp.Value is JsonElement element2 && element2.ValueKind == JsonValueKind.Number)
                    {
                         correctedData[kvp.Key] = element2.GetInt32();
                    }
                    else
                    {
                        correctedData[kvp.Key] = kvp.Value;
                    }
                }
                return correctedData;
            }

            return bagData;
        }

        public static void ListViews()
        {
            var views = LoadViewStore();
            if (!views.Any())
            {
                Console.WriteLine("No views have been saved yet.");
                return;
            }
            
            Console.WriteLine("Saved Views:");
            foreach (var view in views.OrderBy(v => v.Name))
            {
                Console.WriteLine($"- {view.Name} (Saved on: {view.Created.ToLocalTime()})");
            }
        }

        public static void DeleteView(string viewName)
        {
            var views = LoadViewStore();
            var entry = views.FirstOrDefault(v => v.Name.Equals(viewName, StringComparison.OrdinalIgnoreCase));

            if (entry == null)
            {
                Console.Error.WriteLine($"No view found with the name '{viewName}'.");
                return;
            }

            var dataFilePath = Path.Combine(ViewsDir, entry.FileName);
            if (File.Exists(dataFilePath))
            {
                File.Delete(dataFilePath);
            }

            views.Remove(entry);
            SaveViewStore(views);

            Console.WriteLine($"Successfully deleted view '{viewName}'.");
        }

        private static List<ViewStoreEntry> LoadViewStore()
        {
            if (!File.Exists(StoreFileName))
            {
                return new List<ViewStoreEntry>();
            }

            var jsonString = File.ReadAllText(StoreFileName);
            return JsonSerializer.Deserialize<List<ViewStoreEntry>>(jsonString) ?? new List<ViewStoreEntry>();
        }

        private static void SaveViewStore(List<ViewStoreEntry> views)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(views, options);
            File.WriteAllText(StoreFileName, jsonString);
        }
    }
}
