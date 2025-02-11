using Microsoft.Extensions.DependencyInjection;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;

namespace Mo.ProjectFilesToJson.ConsoleApp;

public class AppEngine
{
    private readonly IGitIgnoreService _gitIgnoreService;
    private readonly IFileScanService _fileScanService;
    private readonly IFileFormatService _fileFormatService;
    private readonly ICustomFilterService _customFilterService;
    private readonly string _userSettingsFile;

    public AppEngine(IServiceProvider serviceProvider, string userSettingsFile)
    {
        _gitIgnoreService = serviceProvider.GetRequiredService<IGitIgnoreService>();
        _fileScanService = serviceProvider.GetRequiredService<IFileScanService>();
        _fileFormatService = serviceProvider.GetRequiredService<IFileFormatService>();
        _customFilterService = serviceProvider.GetRequiredService<ICustomFilterService>();
        _userSettingsFile = userSettingsFile;
    }

    public void Run()
    {
        // 1) Load or Prompt for settings
        var lastUsedSettings = Helper.LoadUserSettings(_userSettingsFile);

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
                finalSettings = PromptForSettingsOrAbort();
                if (finalSettings == null) return; // user aborted or no projects found
            }
        }
        else
        {
            finalSettings = PromptForSettingsOrAbort();
            if (finalSettings == null) return;
        }

        // 2) Gather and filter file paths
        var gitIgnorePatterns = _gitIgnoreService.LoadGitIgnorePatterns(finalSettings.ProjectFolderName);
        var allFilePaths = _fileScanService.GetAllFilePaths(finalSettings.SourceFolderPath, gitIgnorePatterns);

        var includePatterns = _customFilterService.LoadIncludePatterns(finalSettings.ProjectFolderName);
        var excludePatterns = _customFilterService.LoadExcludePatterns(finalSettings.ProjectFolderName);
        var filteredPaths = _customFilterService.ApplyPathFilters(allFilePaths, includePatterns, excludePatterns);

        // 3) Read contents
        var fileContents = Helper.ReadFileContents(finalSettings.SourceFolderPath, filteredPaths);

        // 4) Format output
        string output;
        if (finalSettings.FormatIndex == 0)
            output = _fileFormatService.FormatAsJson(fileContents);
        else
            output = _fileFormatService.FormatWithDivider(fileContents);

        // 5) Save output
        try
        {
            File.WriteAllText(finalSettings.DestinationFilePath, output);
            Console.WriteLine($"Successfully saved the result to: {finalSettings.DestinationFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
        }

        // 6) Save settings
        Helper.SaveUserSettings(finalSettings, _userSettingsFile);

        Console.WriteLine("Processing completed.");
    }

    /// <summary>
    /// A small helper method that prompts for new settings and returns them,
    /// or returns null if no projects are found (and user can't continue).
    /// </summary>
    private UserScanSettings? PromptForSettingsOrAbort()
    {
        var newSettings = Helper.PromptForNewSettings(_gitIgnoreService);
        // If PromptForNewSettings returns something invalid or user wants to abort, handle that:
        if (string.IsNullOrWhiteSpace(newSettings.ProjectFolderName))
        {
            return null;
        }
        return newSettings;
    }
}