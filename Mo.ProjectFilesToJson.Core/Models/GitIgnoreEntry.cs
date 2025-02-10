namespace Mo.ProjectFilesToJson.Core.Models;

public class GitIgnoreEntry
{
    public bool IsNegative { get; set; }
    public string Pattern { get; set; } = string.Empty;
}

