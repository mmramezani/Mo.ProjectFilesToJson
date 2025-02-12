namespace Mo.ProjectFilesToJson.Core.Interfaces;

public interface ICustomFilterService
{
    List<string> LoadIncludePatterns(string projectFolderName);
    List<string> LoadExcludePatterns(string projectFolderName);
    List<string> ApplyPathFilters(List<string> filePaths, List<string> includePatterns, List<string> excludePatterns);
    List<string> ApplyGitIgnoreFilters(List<string> filePaths, List<string> gitIgnorePatterns);

}
