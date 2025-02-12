namespace Mo.ProjectFilesToJson.Core.Interfaces;

public interface IGitIgnoreService
{
    List<string> GetAvailableProjects();
    List<string> LoadGitIgnorePatterns(string projectFolderName);
}