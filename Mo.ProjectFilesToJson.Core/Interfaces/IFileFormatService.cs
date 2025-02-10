using Mo.ProjectFilesToJson.Core.Models;

namespace Mo.ProjectFilesToJson.Core.Interfaces;

public interface IFileFormatService
{
    /// <summary>
    /// Formats the given file contents as JSON.
    /// </summary>
    string FormatAsJson(List<FileContent> files);

    /// <summary>
    /// Formats the given file contents using a simple divider format.
    /// </summary>
    string FormatWithDivider(List<FileContent> files);
}