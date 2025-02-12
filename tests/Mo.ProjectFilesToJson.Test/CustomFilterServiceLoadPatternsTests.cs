using FluentAssertions;
using Microsoft.Extensions.Options;
using Mo.ProjectFilesToJson.Core.Models;
using Mo.ProjectFilesToJson.Core.Services;

namespace Mo.ProjectFilesToJson.Test;

public class CustomFilterServiceLoadPatternsTests
{
    [Fact]
    public void LoadIncludePatterns_ProjectFound_ReturnsExpected()
    {
        var settings = Options.Create(new ProjectSettings
        {
            Projects = new System.Collections.Generic.List<ProjectPatternSettings>
                {
                    new ProjectPatternSettings { Name = "X", OnlyIncludePatterns = { "*.cs", "*.json" } }
                }
        });
        var service = new CustomFilterService(settings);
        var result = service.LoadIncludePatterns("X");
        result.Should().HaveCount(2);
        result.Should().Contain("*.cs");
        result.Should().Contain("*.json");
    }

    [Fact]
    public void LoadIncludePatterns_ProjectNotFound_ReturnsEmpty()
    {
        var settings = Options.Create(new ProjectSettings());
        var service = new CustomFilterService(settings);
        var result = service.LoadIncludePatterns("DoesNotExist");
        result.Should().BeEmpty();
    }

    [Fact]
    public void LoadExcludePatterns_ProjectFound_ReturnsExpected()
    {
        var settings = Options.Create(new ProjectSettings
        {
            Projects = new System.Collections.Generic.List<ProjectPatternSettings>
                {
                    new ProjectPatternSettings { Name = "Z", AlsoExcludePatterns = { ".git", "bin/" } }
                }
        });
        var service = new CustomFilterService(settings);
        var result = service.LoadExcludePatterns("Z");
        result.Should().HaveCount(2);
        result.Should().Contain(".git");
        result.Should().Contain("bin/");
    }

    [Fact]
    public void LoadExcludePatterns_ProjectNotFound_ReturnsEmpty()
    {
        var settings = Options.Create(new ProjectSettings());
        var service = new CustomFilterService(settings);
        var result = service.LoadExcludePatterns("Nope");
        result.Should().BeEmpty();
    }
}