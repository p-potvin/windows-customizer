using System.CommandLine;
using ViewManager;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("A helper utility to save and apply folder view settings.");
        var registryManager = new RegistryManager();

        // --- Save Command ---
        var savePathOption = new Option<string>(
            name: "--path",
            description: "The path of the folder to save the view for.")
            { IsRequired = true };
        var saveNameOption = new Option<string>(
            name: "--name",
            description: "A unique name to identify this saved view.")
            { IsRequired = true };

        var saveCommand = new Command("save", "Save the current view settings for a specific folder.")
        {
            savePathOption,
            saveNameOption
        };
        saveCommand.SetHandler((path, name) => 
        {
            Console.WriteLine($"Attempting to save view for '{path}' as '{name}'...");
            var bagId = registryManager.FindBagIdForPath(path);
            if (string.IsNullOrEmpty(bagId))
            {
                Console.Error.WriteLine("Could not find view settings (Bag ID) for the specified path.");
                Console.Error.WriteLine("Please open the folder in Explorer, change the view slightly, and close it to ensure settings are saved by Windows.");
                return;
            }

            Console.WriteLine($"Found Bag ID: {bagId}");
            var bagData = registryManager.GetBagData(bagId);
            if (bagData == null)
            {
                Console.Error.WriteLine($"Could not read data for Bag ID {bagId}.");
                return;
            }

            try
            {
                ViewStore.SaveView(name, bagData);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving view: {ex.Message}");
            }
        }, savePathOption, saveNameOption);

        // --- Apply Command ---
        var applyPathOption = new Option<string>(
            name: "--path",
            description: "The path of the folder to apply the view to.")
            { IsRequired = true };
        var applyNameOption = new Option<string>(
            name: "--name",
            description: "The name of the saved view to apply.")
            { IsRequired = true };
        
        var applyCommand = new Command("apply", "Apply a saved view to a specific folder.")
        {
            applyPathOption,
            applyNameOption
        };
        applyCommand.SetHandler((path, name) => 
        {
            Console.WriteLine($"Attempting to apply view '{name}' to '{path}'...");
            var bagData = ViewStore.LoadViewData(name);
            if (bagData == null) return;

            var bagId = registryManager.FindBagIdForPath(path);
            if (string.IsNullOrEmpty(bagId))
            {
                Console.Error.WriteLine("Could not find view settings (Bag ID) for the target path.");
                Console.Error.WriteLine("Please open the folder in Explorer first to ensure it has an entry in the registry.");
                return;
            }
            
            Console.WriteLine($"Found target Bag ID: {bagId}. Applying settings...");
            registryManager.ApplyBagData(bagId, bagData);
            
            Console.WriteLine("Settings applied. Restarting Explorer to apply changes...");
            registryManager.RestartExplorer();
            Console.WriteLine("Explorer has been restarted.");

        }, applyPathOption, applyNameOption);


        // --- List Command ---
        var listCommand = new Command("list", "List all saved folder views.");
        listCommand.SetHandler(() => 
        {
            ViewStore.ListViews();
        });

        // --- Delete Command ---
        var deleteNameOption = new Option<string>(
            name: "--name",
            description: "The name of the view to delete.")
            { IsRequired = true };

        var deleteCommand = new Command("delete", "Delete a saved folder view.")
        {
            deleteNameOption
        };
        deleteCommand.SetHandler((name) => 
        {
            ViewStore.DeleteView(name);
        }, deleteNameOption);


        rootCommand.AddCommand(saveCommand);
        rootCommand.AddCommand(applyCommand);
        rootCommand.AddCommand(listCommand);
        rootCommand.AddCommand(deleteCommand);

        return await rootCommand.InvokeAsync(args);
    }
}
