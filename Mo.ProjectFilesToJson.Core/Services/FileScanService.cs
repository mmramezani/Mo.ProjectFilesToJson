﻿using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;
using System.Text.RegularExpressions;

namespace Mo.ProjectFilesToJson.Core.Services;

public class FileScanService : IFileScanService
{
    public List<string> GetAllFilePaths(string folderPath, List<string> ignoreLines)
    {
        var result = new List<string>();
        if (!Directory.Exists(folderPath)) return result;

        var gitIgnoreEntries = ParseGitIgnoreEntries(ignoreLines);
        var allFiles = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

        foreach (var file in allFiles)
        {
            var relativePath = file.Substring(folderPath.Length)
                                   .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                                   .Replace("\\", "/");

            if (!IsIgnored(relativePath, gitIgnoreEntries))
            {
                result.Add(relativePath);
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
                ignore = !entry.IsNegative;
            }
        }
        return ignore;
    }

    private bool MatchGitIgnorePattern(string relativePath, string pattern)
    {
        bool matchOnlyRoot = pattern.StartsWith("/");
        if (matchOnlyRoot) pattern = pattern.Substring(1);

        bool directoryPattern = pattern.EndsWith("/");
        if (directoryPattern) pattern = pattern.Substring(0, pattern.Length - 1);

        var regexPattern = GlobToRegex(pattern, directoryPattern, matchOnlyRoot);
        return Regex.IsMatch(relativePath, regexPattern, RegexOptions.IgnoreCase);
    }

    private string GlobToRegex(string pattern, bool directoryPattern, bool matchOnlyRoot)
    {
        string regexSafe = EscapeExceptGlob(pattern);
        regexSafe = regexSafe.Replace("*", "[^/]*").Replace("?", "[^/]");

        string prefix = matchOnlyRoot ? "^" : "^(?:.*/)?";
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

