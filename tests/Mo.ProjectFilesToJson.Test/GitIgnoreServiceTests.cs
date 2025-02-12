using FluentAssertions;
using Microsoft.Extensions.Options;
using Mo.ProjectFilesToJson.Core.Models;
using Mo.ProjectFilesToJson.Core.Services;

namespace Mo.ProjectFilesToJson.Test;

public class GitIgnoreServiceTests
{
    [Fact]
    public void GetAvailableProjects_ShouldReturnDistinctNames()
    {
        var settings = Options.Create(new ProjectSettings
        {
            Projects = new System.Collections.Generic.List<ProjectPatternSettings>
                {
                    new ProjectPatternSettings { Name = "ProjA" },
                    new ProjectPatternSettings { Name = "ProjB" },
                    new ProjectPatternSettings { Name = "ProjA" }
                }
        });
        var service = new GitIgnoreService(settings);
        var result = service.GetAvailableProjects();
        result.Should().HaveCount(2);
        result.Should().Contain(new[] { "ProjA", "ProjB" });
    }

    [Fact]
    public void LoadGitIgnorePatterns_NoFolder_WarnsAndEmpty()
    {
        var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var settings = Options.Create(new ProjectSettings
        {
            ProjectGitsFileFolder = folderPath,
            Projects = new System.Collections.Generic.List<ProjectPatternSettings>()
        });
        var service = new GitIgnoreService(settings);

        using var sw = new StringWriter();
        Console.SetOut(sw);

        var result = service.LoadGitIgnorePatterns("SomeProject");
        result.Should().BeEmpty();
        sw.ToString().Should().Contain("does not exist");
    }

    [Fact]
    public void LoadGitIgnorePatterns_NoGitIgnore_WarnsAndEmpty()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(baseDir);
        var settings = Options.Create(new ProjectSettings
        {
            ProjectGitsFileFolder = baseDir,
            Projects = new System.Collections.Generic.List<ProjectPatternSettings>
                {
                    new ProjectPatternSettings { Name = "ProjX" }
                }
        });
        var service = new GitIgnoreService(settings);

        var projDir = Path.Combine(baseDir, "ProjX");
        Directory.CreateDirectory(projDir);

        using var sw = new StringWriter();
        Console.SetOut(sw);

        var result = service.LoadGitIgnorePatterns("ProjX");
        result.Should().BeEmpty();
        sw.ToString().Should().Contain("No .gitignore found");

        Directory.Delete(baseDir, true);
    }

    [Fact]
    public void LoadGitIgnorePatterns_ValidGitIgnore_ReturnsLines()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(baseDir);
        var settings = Options.Create(new ProjectSettings
        {
            ProjectGitsFileFolder = baseDir,
            Projects = new System.Collections.Generic.List<ProjectPatternSettings>
                {
                    new ProjectPatternSettings { Name = "ProjX" }
                }
        });
        var service = new GitIgnoreService(settings);

        var projDir = Path.Combine(baseDir, "ProjX");
        Directory.CreateDirectory(projDir);
        var gitIgnorePath = Path.Combine(projDir, ".gitignore");
        File.WriteAllLines(gitIgnorePath, new[]
        {
                "# comment",
                "",
                "bin/",
                "!bin/special.dll"
            });

        using var sw = new StringWriter();
        Console.SetOut(sw);

        var result = service.LoadGitIgnorePatterns("ProjX");
        result.Should().HaveCount(2);
        result.Should().Contain("bin/");
        result.Should().Contain("!bin/special.dll");

        Directory.Delete(baseDir, true);
    }
}