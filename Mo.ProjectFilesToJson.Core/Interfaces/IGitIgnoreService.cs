namespace Mo.ProjectFilesToJson.Core.Interfaces;

public interface IGitIgnoreService
{
    List<string> GetAvailableProjects();
    List<string> LoadGitIgnorePatterns(List<string> gitIgnoreRelativePaths, string baseFolderPath);
}