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
            Console.WriteLine("Do you want to continue with these settings? (Y/N) [Enter = Yes]");
            var choice = Console.ReadLine()?.Trim().ToLower();
            if (string.IsNullOrEmpty(choice) || choice == "y" || choice == "yes")
            {
                finalSettings = lastUsedSettings;
            }
            else
            {
                finalSettings = PromptForSettingsOrAbort();
                if (finalSettings == null) return;
            }
        }
        else
        {
            finalSettings = PromptForSettingsOrAbort();
            if (finalSettings == null) return;
        }

        // 2) Gather ALL file paths (unfiltered) + .gitignore file paths
        var (allFiles, gitIgnoreFiles) = _fileScanService.GetAllFilePaths(finalSettings.SourceFolderPath);

        // 3) If we have .gitignore files, read them and apply .gitignore rules
        var gitIgnorePatterns = _gitIgnoreService.LoadGitIgnorePatterns(
            gitIgnoreFiles,
            finalSettings.SourceFolderPath
        );
        var afterGitIgnore = _customFilterService.ApplyGitIgnoreFilters(allFiles, gitIgnorePatterns);

        // 4) Now apply your custom config-based filters
        var includePatterns = _customFilterService.LoadIncludePatterns(finalSettings.ProjectFolderName);
        var excludePatterns = _customFilterService.LoadExcludePatterns(finalSettings.ProjectFolderName);
        var filteredPaths = _customFilterService.ApplyPathFilters(afterGitIgnore, includePatterns, excludePatterns);

        // 5) Read the remaining file contents
        var fileContents = Helper.ReadFileContents(finalSettings.SourceFolderPath, filteredPaths);

        // 6) Format output
        string output;
        if (finalSettings.FormatIndex == 0)
            output = _fileFormatService.FormatAsJson(fileContents);
        else
            output = _fileFormatService.FormatWithDivider(fileContents);

        // 7) Save to disk
        try
        {
            File.WriteAllText(finalSettings.DestinationFilePath, output);
            Console.WriteLine($"Successfully saved the result to: {finalSettings.DestinationFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
        }

        // 8) Save user settings
        Helper.SaveUserSettings(finalSettings, _userSettingsFile);
        Console.WriteLine("Processing completed.");
    }

    private UserScanSettings? PromptForSettingsOrAbort()
    {
        var newSettings = Helper.PromptForNewSettings(_gitIgnoreService);
        if (string.IsNullOrWhiteSpace(newSettings.ProjectFolderName))
            return null;
        return newSettings;
    }
}