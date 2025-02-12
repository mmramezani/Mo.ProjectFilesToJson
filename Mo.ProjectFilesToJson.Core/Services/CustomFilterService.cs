using Microsoft.Extensions.Options;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;
using System.Text.RegularExpressions;

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
        var project = _scannerSettings.Projects
            .FirstOrDefault(x => x.Name.Equals(projectFolderName, StringComparison.OrdinalIgnoreCase));

        return project?.OnlyIncludePatterns ?? new List<string>();
    }

    public List<string> LoadExcludePatterns(string projectFolderName)
    {
        var project = _scannerSettings.Projects
            .FirstOrDefault(x => x.Name.Equals(projectFolderName, StringComparison.OrdinalIgnoreCase));

        return project?.AlsoExcludePatterns ?? new List<string>();
    }

    public List<string> ApplyPathFilters(
        List<string> filePaths,
        List<string> includePatterns,
        List<string> excludePatterns)
    {
        // EXACTLY what you had before
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

        // Your existing code for custom patterns
        if (pattern.StartsWith("*.", StringComparison.Ordinal))
        {
            var ext = Path.GetExtension(normPath);
            var patternExt = pattern.Substring(1);
            return ext.Equals(patternExt, StringComparison.OrdinalIgnoreCase);
        }

        var dotParts = pattern.Split('.');
        if (dotParts.Length == 2 &&
            !string.IsNullOrWhiteSpace(dotParts[0]) &&
            !string.IsNullOrWhiteSpace(dotParts[1]))
        {
            var fileName = Path.GetFileName(normPath);
            if (fileName.Equals(pattern, StringComparison.OrdinalIgnoreCase))
                return true;
        }

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

    // ---------------------------------------------------------------------
    // NEW: .gitignore logic
    // ---------------------------------------------------------------------
    public List<string> ApplyGitIgnoreFilters(List<string> filePaths, List<string> gitIgnorePatterns)
    {
        // If no .gitignore, return the original list
        if (gitIgnorePatterns == null || gitIgnorePatterns.Count == 0)
            return filePaths;

        // Convert each line into a GitIgnoreEntry
        var entries = ParseGitIgnoreEntries(gitIgnorePatterns);

        var result = new List<string>();
        foreach (var path in filePaths)
        {
            if (!IsIgnored(path, entries))
            {
                // Keep it only if NOT ignored
                result.Add(path);
            }
        }
        return result;
    }

    private List<GitIgnoreEntry> ParseGitIgnoreEntries(List<string> lines)
    {
        var entries = new List<GitIgnoreEntry>();
        foreach (var line in lines)
        {
            if (line.StartsWith("!"))
            {
                entries.Add(new GitIgnoreEntry
                {
                    IsNegative = true,
                    Pattern = line.Substring(1)
                });
            }
            else
            {
                entries.Add(new GitIgnoreEntry
                {
                    IsNegative = false,
                    Pattern = line
                });
            }
        }
        return entries;
    }

    private bool IsIgnored(string relativePath, List<GitIgnoreEntry> entries)
    {
        bool ignore = false;
        foreach (var entry in entries)
        {
            if (MatchGitIgnorePattern(relativePath, entry.Pattern))
            {
                // If we match a negative pattern (e.g. `!something`), 
                // that means "un-ignore", so we flip it back
                ignore = !entry.IsNegative;
            }
        }
        return ignore;
    }

    private bool MatchGitIgnorePattern(string relativePath, string pattern)
    {
        bool matchOnlyRoot = pattern.StartsWith("/");
        if (matchOnlyRoot)
            pattern = pattern.Substring(1);

        bool directoryPattern = pattern.EndsWith("/");
        if (directoryPattern)
            pattern = pattern.Substring(0, pattern.Length - 1);

        var regexPattern = GlobToRegex(pattern, directoryPattern, matchOnlyRoot);
        return Regex.IsMatch(relativePath, regexPattern, RegexOptions.IgnoreCase);
    }

    private string GlobToRegex(string pattern, bool directoryPattern, bool matchOnlyRoot)
    {
        // Convert * and ? to equivalent regex
        string regexSafe = EscapeExceptGlob(pattern);
        regexSafe = regexSafe
            .Replace("*", "[^/]*")
            .Replace("?", "[^/]");

        // Anchor pattern to start if needed
        string prefix = matchOnlyRoot ? "^" : "^(?:.*/)?";
        // If it's a directory pattern, match anything below it
        string suffix = directoryPattern ? "(?:/.*)?$" : "$";

        return prefix + regexSafe + suffix;
    }

    private string EscapeExceptGlob(string pattern)
    {
        var toEscape = new char[] { '.', '^', '$', '+', '=', ':', '{', '}', '(', ')', '|', '\\' };
        var sb = new System.Text.StringBuilder();
        foreach (var ch in pattern)
        {
            if (toEscape.Contains(ch))
            {
                sb.Append('\\').Append(ch);
            }
            else
            {
                sb.Append(ch);
            }
        }
        return sb.ToString();
    }
}