namespace Mo.ProjectFilesToJson.Core.Interfaces;

public interface IFileScanService
{
    List<string> GetAllFilePaths(string folderPath, List<string> ignoreLines);
}
