using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mo.ProjectFilesToJson.ConsoleApp;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;
using Mo.ProjectFilesToJson.Core.Services;


internal class Program
{
    private const string USER_SETTINGS_FILE = "userSettings.json";

    static void Main(string[] args)
    {
        // 1. Build configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfiguration configuration = builder.Build();

        // 2. Create ServiceCollection, pass in configuration
        var services = new ServiceCollection();

        // 3. Bind ProjectScannerSettings from config
        //    Then register it as a singleton for injection
        services.Configure<ProjectScannerSettings>(configuration.GetSection("ProjectScannerSettings"));

        // 4. Register the rest of your services
        //    (You can do this in a separate method if you like, or inline)
        services.AddSingleton<IGitIgnoreService, GitIgnoreService>();
        services.AddSingleton<IFileScanService, FileScanService>();
        services.AddSingleton<IFileFormatService, FileFormatService>();
        services.AddSingleton<ICustomFilterService, CustomFilterService>();

        var serviceProvider = services.BuildServiceProvider();

        // now retrieve your services
        var gitIgnoreService = serviceProvider.GetService<IGitIgnoreService>();
        var fileScanService = serviceProvider.GetService<IFileScanService>();
        var fileFormatService = serviceProvider.GetService<IFileFormatService>();
        var customFilterService = serviceProvider.GetService<ICustomFilterService>();

        // 5. The rest of your existing Program logic remains mostly the same
        var lastUsedSettings = Helper.LoadUserSettings(USER_SETTINGS_FILE);

        UserScanSettings finalSettings;
        if (lastUsedSettings != null)
        {
            Console.WriteLine("Previous settings found:");
            Helper.PrintUserSettings(lastUsedSettings);
            Console.WriteLine("Do you want to continue with these settings? (Y/N)");

            var choice = Console.ReadLine()?.Trim().ToLower();
            if (choice == "y" || choice == "yes")
            {
                finalSettings = lastUsedSettings;
            }
            else
            {
                finalSettings = Helper.PromptForNewSettings(gitIgnoreService);
            }
        }
        else
        {
            finalSettings = Helper.PromptForNewSettings(gitIgnoreService);
        }

        var gitIgnorePatterns = gitIgnoreService!.LoadGitIgnorePatterns(finalSettings.ProjectFolderName);
        var allFilePaths = fileScanService!.GetAllFilePaths(finalSettings.SourceFolderPath, gitIgnorePatterns);

        var includePatterns = customFilterService!.LoadIncludePatterns(finalSettings.ProjectFolderName);
        var excludePatterns = customFilterService.LoadExcludePatterns(finalSettings.ProjectFolderName);
        var filteredPaths = customFilterService.ApplyPathFilters(allFilePaths, includePatterns, excludePatterns);

        var fileContents = Helper.ReadFileContents(finalSettings.SourceFolderPath, filteredPaths);

        string output;
        if (finalSettings.FormatIndex == 0)
            output = fileFormatService!.FormatAsJson(fileContents);
        else
            output = fileFormatService!.FormatWithDivider(fileContents);

        try
        {
            File.WriteAllText(finalSettings.DestinationFilePath, output);
            Console.WriteLine($"Successfully saved the result to: {finalSettings.DestinationFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
        }

        Helper.SaveUserSettings(finalSettings, USER_SETTINGS_FILE);
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}