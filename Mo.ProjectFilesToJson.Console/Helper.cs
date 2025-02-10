using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;
using Mo.ProjectFilesToJson.Core.Services;

namespace Mo.ProjectFilesToJson.ConsoleApp;

public static class Helper
{
    public static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IGitIgnoreService, GitIgnoreService>();
        services.AddSingleton<IFileScanService, FileScanService>();
        services.AddSingleton<IFileFormatService, FileFormatService>();
        services.AddSingleton<ICustomFilterService, CustomFilterService>();

        return services.BuildServiceProvider();
    }

    public static UserScanSettings? LoadUserSettings(string filePath)
    {
        if (!File.Exists(filePath)) return null;
        try
        {
            var json = File.ReadAllText(filePath);
            var settings = JsonConvert.DeserializeObject<UserScanSettings>(json);
            if (settings == null) return null;
            if (string.IsNullOrEmpty(settings.ProjectFolderName)
                || string.IsNullOrEmpty(settings.SourceFolderPath)
                || string.IsNullOrEmpty(settings.DestinationFilePath))
            {
                return null;
            }
            return settings;
        }
        catch
        {
            return null;
        }
    }

    public static void SaveUserSettings(UserScanSettings settings, string filePath)
    {
        try
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save user settings: {ex.Message}");
        }
    }

    public static void PrintUserSettings(UserScanSettings settings)
    {
        Console.WriteLine("  Project Folder: " + settings.ProjectFolderName);
        Console.WriteLine("  Source Folder:  " + settings.SourceFolderPath);
        Console.WriteLine("  Destination:    " + settings.DestinationFilePath);
        Console.WriteLine("  Format:         " + (settings.FormatIndex == 0 ? "JSON" : "Simple Divider"));
    }

    public static UserScanSettings PromptForNewSettings(IGitIgnoreService gitIgnoreService)
    {
        var settings = new UserScanSettings();
        var projectFolders = gitIgnoreService.GetAvailableProjects();
        if (projectFolders.Count == 0)
        {
            Console.WriteLine("No project folders found. Exiting...");
            Environment.Exit(0);
        }

        Console.WriteLine("Select your source project (by number):");
        for (int i = 0; i < projectFolders.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {projectFolders[i]}");
        }
        int projectIndex = PromptUserForIndex(projectFolders.Count);
        settings.ProjectFolderName = projectFolders[projectIndex];

        Console.WriteLine("Enter the **absolute source folder path** (e.g. D:\\MyProjects\\AwesomeApp):");
        settings.SourceFolderPath = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Enter the **destination path** (folder or file). If folder only, will use 'Result.txt':");
        var destinationInput = Console.ReadLine() ?? string.Empty;
        settings.DestinationFilePath = BuildOutputFilePath(destinationInput);

        Console.WriteLine("Select output text format:");
        Console.WriteLine("  1. JSON");
        Console.WriteLine("  2. Simple divider");
        int formatIndex = PromptUserForIndex(2);
        settings.FormatIndex = formatIndex;

        return settings;
    }

    public static List<FileContent> ReadFileContents(string basePath, List<string> filteredPaths)
    {
        var list = new List<FileContent>();
        foreach (var relativePath in filteredPaths)
        {
            var fullPath = Path.Combine(basePath, relativePath.TrimStart('/', '\\'));
            if (File.Exists(fullPath))
            {
                var content = File.ReadAllText(fullPath);
                list.Add(new FileContent
                {
                    FilePath = relativePath,
                    Content = content
                });
            }
        }
        return list;
    }

    public static string BuildOutputFilePath(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            var currentDir = Directory.GetCurrentDirectory();
            return Path.Combine(currentDir, "Result.txt");
        }

        if (Path.HasExtension(input))
        {
            return Path.GetFullPath(input);
        }
        else
        {
            var fullDirPath = Path.GetFullPath(input);
            return Path.Combine(fullDirPath, "Result.txt");
        }
    }

    private static int PromptUserForIndex(int count)
    {
        while (true)
        {
            Console.Write("Enter choice: ");
            var input = Console.ReadLine();
            if (int.TryParse(input, out int selection) && selection > 0 && selection <= count)
            {
                return selection - 1;
            }
            Console.WriteLine("Invalid selection. Try again.");
        }
    }
}