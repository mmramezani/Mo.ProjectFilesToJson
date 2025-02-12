namespace Mo.ProjectFilesToJson.Core.Models;

public class ProjectSettings
{
    public string ProjectGitsFileFolder { get; set; } = string.Empty;
    public List<ProjectPatternSettings> Projects { get; set; } = new List<ProjectPatternSettings>();
}