namespace Mo.ProjectFilesToJson.Core.Models;

public class ProjectPatternSettings
{
    public string Name { get; set; } = string.Empty;
    public List<string> OnlyIncludePatterns { get; set; } = new List<string>();
    public List<string> AlsoExcludePatterns { get; set; } = new List<string>();
}