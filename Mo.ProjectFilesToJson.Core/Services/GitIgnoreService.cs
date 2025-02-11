using Microsoft.Extensions.Options;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;

namespace Mo.ProjectFilesToJson.Core.Services;

public class GitIgnoreService : IGitIgnoreService
{
    private readonly string _projectGitsFileFolder;

    public GitIgnoreService(IOptions<ProjectScannerSettings> settings)
    {
        var folder = settings.Value.ProjectGitsFileFolder;
        if (!Path.IsPathRooted(folder))
        {
            // Resolve it relative to the current directory (or any other base you prefer)
            folder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), folder));
        }
        _projectGitsFileFolder = folder;
    }

    public List<string> GetAvailableProjects()
    {
        if (!Directory.Exists(_projectGitsFileFolder))
            return new List<string>();

        var directories = Directory.GetDirectories(_projectGitsFileFolder);
        return directories.Select(Path.GetFileName).ToList();
    }

    public List<string> LoadGitIgnorePatterns(string projectFolderName)
    {
        var patterns = new List<string>();
        string gitIgnorePath = Path.Combine(_projectGitsFileFolder, projectFolderName, ".gitignore");
        if (!File.Exists(gitIgnorePath))
            return patterns;

        var lines = File.ReadAllLines(gitIgnorePath);
        foreach (var line in lines)
        {
            var trimLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimLine)) continue;
            if (trimLine.StartsWith("#")) continue;
            patterns.Add(trimLine);
        }
        return patterns;
    }
}