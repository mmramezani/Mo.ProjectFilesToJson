namespace Mo.ProjectFilesToJson.Core.Interfaces;

public interface IGitIgnoreService
{
    /// <summary>
    /// Gets the names of all available project folders in "ProjectGitsFile" directory.
    /// </summary>
    List<string> GetAvailableProjects();

    /// <summary>
    /// Loads .gitignore patterns from the specified project folder in "ProjectGitsFile".
    /// </summary>
    List<string> LoadGitIgnorePatterns(string projectFolderName);
}