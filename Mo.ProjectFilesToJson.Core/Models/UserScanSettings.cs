namespace Mo.ProjectFilesToJson.Core.Models;

public class UserScanSettings
{
    public string ProjectFolderName { get; set; } = string.Empty;
    public string SourceFolderPath { get; set; } = string.Empty;
    public string DestinationFilePath { get; set; } = string.Empty;
    /// <summary>
    /// 0 => JSON, 1 => Simple Divider
    /// </summary>
    public int FormatIndex { get; set; }
}