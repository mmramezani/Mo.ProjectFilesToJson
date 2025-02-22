﻿using System.Text.RegularExpressions;

namespace Mo.ProjectFilesToJson.Core.Services;

public static class GitIgnoreParser
{
    public static bool IsIgnored(string relativePath, List<string> patterns)
    {
        foreach (var pattern in patterns)
        {
            if (PatternMatches(relativePath, pattern))
            {
                return true;
            }
        }
        return false;
    }

    private static bool PatternMatches(string relativePath, string pattern)
    {
        // If pattern ends with "/", treat it as a folder ignore
        if (pattern.EndsWith("/"))
        {
            // Check if relativePath starts with that folder
            // e.g. pattern = "bin/", relativePath = "bin/Debug/something.dll"
            return relativePath.StartsWith(pattern.TrimEnd('/'));
        }

        // If pattern starts with "*."
        if (pattern.StartsWith("*."))
        {
            // e.g. pattern = "*.exe"
            var ext = Path.GetExtension(relativePath);
            var patternExt = pattern.Substring(1); // ".exe"
            return ext.Equals(patternExt, StringComparison.OrdinalIgnoreCase);
        }

        // If pattern has no wildcard, check direct equality
        if (!pattern.Contains("*"))
        {
            // e.g. "obj/temp.txt"
            return relativePath.Equals(pattern, StringComparison.OrdinalIgnoreCase);
        }

        // Simple wildcard approach: convert pattern to a regex
        var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
        return Regex.IsMatch(relativePath, regexPattern, RegexOptions.IgnoreCase);
    }
}