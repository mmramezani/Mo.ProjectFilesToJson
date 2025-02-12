using Microsoft.Extensions.Options;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;

namespace Mo.ProjectFilesToJson.Core.Services;

public class CustomFilterService : ICustomFilterService
{
    private readonly ProjectSettings _scannerSettings;

    public CustomFilterService(IOptions<ProjectSettings> options)
    {
        _scannerSettings = options.Value;
    }

    public List<string> LoadIncludePatterns(string projectFolderName)
    {
        // Try to find the project in the list of configured Projects
        var project = _scannerSettings.Projects
            .FirstOrDefault(x => x.Name.Equals(projectFolderName, StringComparison.OrdinalIgnoreCase));

        if (project != null)
        {
            return project.OnlyIncludePatterns ?? new List<string>();
        }

        return new List<string>();
    }

    public List<string> LoadExcludePatterns(string projectFolderName)
    {
        // Try to find the project in the list of configured Projects
        var project = _scannerSettings.Projects
            .FirstOrDefault(x => x.Name.Equals(projectFolderName, StringComparison.OrdinalIgnoreCase));

        if (project != null)
        {
            return project.AlsoExcludePatterns ?? new List<string>();
        }

        return new List<string>();
    }

    public List<string> ApplyPathFilters(
        List<string> filePaths,
        List<string> includePatterns,
        List<string> excludePatterns)
    {
        var filtered = new List<string>(filePaths);

        // If we have any include patterns, keep only those matching them:
        if (includePatterns.Count > 0)
        {
            filtered = filtered
                .Where(p => MatchesAnyPattern(p, includePatterns))
                .ToList();
        }

        // Then exclude any that match the exclude patterns
        if (excludePatterns.Count > 0)
        {
            filtered = filtered
                .Where(p => !MatchesAnyPattern(p, excludePatterns))
                .ToList();
        }

        return filtered;
    }

    private bool MatchesAnyPattern(string filePath, List<string> patterns)
    {
        foreach (var pat in patterns)
        {
            if (IsMatch(filePath, pat))
                return true;
        }
        return false;
    }

    private bool IsMatch(string filePath, string pattern)
    {
        var normPath = filePath.Replace("\\", "/");

        // 1) If pattern starts with "*." (like "*.exe"), check file extension
        if (pattern.StartsWith("*.", StringComparison.Ordinal))
        {
            var ext = Path.GetExtension(normPath);
            var patternExt = pattern.Substring(1);
            return ext.Equals(patternExt, StringComparison.OrdinalIgnoreCase);
        }

        // 2) If pattern is "filename.ext"
        var dotParts = pattern.Split('.');
        if (dotParts.Length == 2 &&
            !string.IsNullOrWhiteSpace(dotParts[0]) &&
            !string.IsNullOrWhiteSpace(dotParts[1]))
        {
            var fileName = Path.GetFileName(normPath);
            if (fileName.Equals(pattern, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        // 3) Otherwise, do a segment-based match
        var segments = normPath.Split('/');
        foreach (var seg in segments)
        {
            if (seg.Equals(pattern, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}