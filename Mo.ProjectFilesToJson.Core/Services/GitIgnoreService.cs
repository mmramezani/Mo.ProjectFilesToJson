using Microsoft.Extensions.Options;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;

namespace Mo.ProjectFilesToJson.Core.Services;

public class GitIgnoreService : IGitIgnoreService
{
    private readonly ProjectSettings _scannerSettings;

    public GitIgnoreService(IOptions<ProjectSettings> settings)
    {
        _scannerSettings = settings.Value;
    }

    public List<string> GetAvailableProjects()
    {
        return _scannerSettings.Projects
            .Select(p => p.Name)
            .Distinct()
            .ToList();
    }

    public List<string> LoadGitIgnorePatterns(List<string> gitIgnoreRelativePaths, string baseFolderPath)
    {
        var combinedPatterns = new List<string>();

        if (gitIgnoreRelativePaths == null || gitIgnoreRelativePaths.Count == 0)
            return combinedPatterns;

        foreach (var relativePath in gitIgnoreRelativePaths)
        {
            var fullPath = Path.Combine(baseFolderPath, relativePath);
            if (!File.Exists(fullPath))
                continue;

            var lines = File.ReadAllLines(fullPath);
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                // Skip empty lines or comment lines
                if (string.IsNullOrWhiteSpace(trimmed)) continue;
                if (trimmed.StartsWith("#")) continue;

                combinedPatterns.Add(trimmed);
            }
        }

        return combinedPatterns;
    }
}