using Microsoft.Extensions.Options;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;

namespace Mo.ProjectFilesToJson.Core.Services;

public class GitIgnoreService : IGitIgnoreService
{
    private readonly ProjectSettings _scannerSettings;
    private readonly string _projectGitsFileFolder;

    public GitIgnoreService(IOptions<ProjectSettings> settings)
    {
        _scannerSettings = settings.Value;

        var folder = _scannerSettings.ProjectGitsFileFolder;
        if (!Path.IsPathRooted(folder))
        {
            folder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), folder));
        }
        _projectGitsFileFolder = folder;
    }

    public List<string> GetAvailableProjects()
    {
        return _scannerSettings.Projects
            .Select(p => p.Name)
            .Distinct()
            .ToList();
    }

    public List<string> LoadGitIgnorePatterns(string projectFolderName)
    {
        var results = new List<string>();
        var projectPath = Path.Combine(_projectGitsFileFolder, projectFolderName);

        if (!Directory.Exists(projectPath))
        {
            Console.WriteLine($"[Warning] The folder '{projectPath}' does not exist. Skipping .gitignore loading...");
            return results;
        }

        var gitIgnorePath = Path.Combine(projectPath, ".gitignore");
        if (!File.Exists(gitIgnorePath))
        {
            Console.WriteLine($"[Warning] No .gitignore found for '{projectFolderName}' in '{projectPath}'. Skipping .gitignore loading...");
            return results;
        }

        var lines = File.ReadAllLines(gitIgnorePath);
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;
            if (trimmed.StartsWith("#")) continue;
            results.Add(trimmed);
        }

        return results;
    }
}