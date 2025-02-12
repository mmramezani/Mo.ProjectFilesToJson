namespace Mo.ProjectFilesToJson.Core.Interfaces;

public interface IFileScanService
{
    (List<string> AllFiles, List<string> GitIgnoreFiles) GetAllFilePaths(string folderPath);
}
