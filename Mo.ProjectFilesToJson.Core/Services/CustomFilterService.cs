using Mo.ProjectFilesToJson.Core.Interfaces;

namespace Mo.ProjectFilesToJson.Core.Services;

public class CustomFilterService : ICustomFilterService
{
    private const string PROJECT_GITS_FILE_FOLDER = @"G:\Projects\Mo.ProjectFilesToJson\Mo.ProjectFilesToJson.Console\ProjectGitsFile";
    private const string INCLUDE_FILE_AND_FOLDERS = "OnlyInclude.txt";
    private const string EXCLUDE_FILE_AND_FOLDERS = "AlsoExclude.txt";

    public List<string> LoadIncludePatterns(string projectFolderName)
    {
        string path = Path.Combine(PROJECT_GITS_FILE_FOLDER, projectFolderName, INCLUDE_FILE_AND_FOLDERS);
        return LoadPatternsFromFile(path);
    }

    public List<string> LoadExcludePatterns(string projectFolderName)
    {
        string path = Path.Combine(PROJECT_GITS_FILE_FOLDER, projectFolderName, EXCLUDE_FILE_AND_FOLDERS);
        return LoadPatternsFromFile(path);
    }

    public List<string> ApplyPathFilters(List<string> filePaths, List<string> includePatterns, List<string> excludePatterns)
    {
        var filtered = new List<string>(filePaths);

        if (includePatterns.Count > 0)
        {
            filtered = filtered.Where(p => MatchesAnyPattern(p, includePatterns)).ToList();
        }
        if (excludePatterns.Count > 0)
        {
            filtered = filtered.Where(p => !MatchesAnyPattern(p, excludePatterns)).ToList();
        }

        return filtered;
    }

    private List<string> LoadPatternsFromFile(string filePath)
    {
        var patterns = new List<string>();
        if (!File.Exists(filePath)) return patterns;

        var allText = File.ReadAllText(filePath);
        var separators = new[] { '\r', '\n', ',' };
        var tokens = allText.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

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

        if (pattern.StartsWith("*.", System.StringComparison.Ordinal))
        {
            var ext = Path.GetExtension(normPath);
            var patternExt = pattern.Substring(1);
            return ext.Equals(patternExt, System.StringComparison.OrdinalIgnoreCase);
        }

        // Split the pattern on '.' and check if we have exactly two parts
        var dotParts = pattern.Split('.');
        if (dotParts.Length == 2 &&
            !string.IsNullOrWhiteSpace(dotParts[0]) &&
            !string.IsNullOrWhiteSpace(dotParts[1]))
        {
            // We have something like "filename.ext"
            var fileName = Path.GetFileName(normPath);
            return fileName.Equals(pattern, StringComparison.OrdinalIgnoreCase);
        }

        var segments = normPath.Split('/');
        foreach (var seg in segments)
        {
            if (seg.Equals(pattern, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}