using Newtonsoft.Json;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;
using System.Text;

namespace Mo.ProjectFilesToJson.Core.Services;

public class FileFormatService : IFileFormatService
{
    public string FormatAsJson(List<FileContent> files)
    {
        return JsonConvert.SerializeObject(files);
    }

    public string FormatWithDivider(List<FileContent> files)
    {
        var sb = new StringBuilder();
        foreach (var fc in files)
        {
            var fileName = Path.GetFileName(fc.FilePath);
            sb.AppendLine($"--FILE {fc.FilePath}");
            sb.AppendLine(fc.Content);
            sb.AppendLine($"--END ({fileName})");
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
