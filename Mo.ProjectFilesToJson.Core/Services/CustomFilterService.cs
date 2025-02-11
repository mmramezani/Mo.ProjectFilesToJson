using Microsoft.Extensions.Options;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;

namespace Mo.ProjectFilesToJson.Core.Services;

public class CustomFilterService : ICustomFilterService
{
    private readonly string _projectGitsFileFolder;
    private const string INCLUDE_FILE_AND_FOLDERS = "OnlyInclude.txt";
    private const string EXCLUDE_FILE_AND_FOLDERS = "AlsoExclude.txt";

    public CustomFilterService(IOptions<ProjectScannerSettings> settings)
    {
        var folder = settings.Value.ProjectGitsFileFolder;
        if (!Path.IsPathRooted(folder))
        {
            folder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), folder));
        }
        _projectGitsFileFolder = folder;
    }


    public List<string> LoadIncludePatterns(string projectFolderName)
    {
        string path = Path.Combine(_projectGitsFileFolder, projectFolderName, INCLUDE_FILE_AND_FOLDERS);
        return LoadPatternsFromFile(path);
    }

    public List<string> LoadExcludePatterns(string projectFolderName)
    {
        string path = Path.Combine(_projectGitsFileFolder, projectFolderName, EXCLUDE_FILE_AND_FOLDERS);
        return LoadPatternsFromFile(path);
    }

    public List<string> ApplyPathFilters(List<string> filePaths, List<string> includePatterns, List<string> excludePatterns)
    {
        var filtered = new List<string>(filePaths);

        if (includePatterns.Count > 0)
        {
            filtered = filtered
                .Where(p => MatchesAnyPattern(p, includePatterns))
                .ToList();
        }

        if (excludePatterns.Count > 0)
        {
            filtered = filtered
                .Where(p => !MatchesAnyPattern(p, excludePatterns))
                .ToList();
        }

        return filtered;
    }

    private List<string> LoadPatternsFromFile(string filePath)
    {
        var patterns = new List<string>();
        if (!File.Exists(filePath)) return patterns;

        var allText = File.ReadAllText(filePath);
        var separators = new[] { '\r', '\n', ',' };
        var tokens = allText.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        foreach (var raw in tokens)
        {
            var trim = raw.Trim();
            if (!string.IsNullOrWhiteSpace(trim))
            {
                patterns.Add(trim);
            }
        }
        return patterns;
    }

    private bool MatchesAnyPattern(string filePath, List<string> patterns)
    {
        foreach (var pat in patterns)
        {
            if (IsMatch(filePath, pat)) return true;
        }
        return false;
    }

    private bool IsMatch(string filePath, string pattern)
    {
        var normPath = filePath.Replace("\\", "/");

        if (pattern.StartsWith("*.", StringComparison.Ordinal))
        {
            var ext = Path.GetExtension(normPath);
            var patternExt = pattern.Substring(1);
            return ext.Equals(patternExt, StringComparison.OrdinalIgnoreCase);
        }

        // If pattern is "filename.ext"
        var dotParts = pattern.Split('.');
        if (dotParts.Length == 2 &&
            !string.IsNullOrWhiteSpace(dotParts[0]) &&
            !string.IsNullOrWhiteSpace(dotParts[1]))
        {
            var fileName = Path.GetFileName(normPath);
            return fileName.Equals(pattern, StringComparison.OrdinalIgnoreCase);
        }

        // Otherwise, just do a segment-based match
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