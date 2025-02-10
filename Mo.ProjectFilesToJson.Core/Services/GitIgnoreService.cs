using Mo.ProjectFilesToJson.Core.Interfaces;

namespace Mo.ProjectFilesToJson.Core.Services;

public class GitIgnoreService : IGitIgnoreService
{
    private const string PROJECT_GITS_FILE_FOLDER = @"G:\Projects\Mo.ProjectFilesToJson\Mo.ProjectFilesToJson.Console\ProjectGitsFile";

    public List<string> GetAvailableProjects()
    {
        if (!Directory.Exists(PROJECT_GITS_FILE_FOLDER)) return new List<string>();
        var directories = Directory.GetDirectories(PROJECT_GITS_FILE_FOLDER);
        return directories.Select(Path.GetFileName).ToList();
    }

    public List<string> LoadGitIgnorePatterns(string projectFolderName)
    {
        var patterns = new List<string>();
        string gitIgnorePath = Path.Combine(PROJECT_GITS_FILE_FOLDER, projectFolderName, ".gitignore");
        if (!File.Exists(gitIgnorePath)) return patterns;

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