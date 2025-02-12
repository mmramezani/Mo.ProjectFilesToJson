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
                    new ProjectPatternSettings { Name = "ProjA" } // duplicate
                }
        });
        var service = new GitIgnoreService(settings);

        var result = service.GetAvailableProjects();

        result.Should().HaveCount(2);
        result.Should().Contain(new[] { "ProjA", "ProjB" });
    }

    [Fact]
    public void LoadGitIgnorePatterns_NullList_ShouldReturnEmpty()
    {
        // Arrange
        var settings = Options.Create(new ProjectSettings());
        var service = new GitIgnoreService(settings);

        // Act
        var result = service.LoadGitIgnorePatterns(null, "C:\\BasePath");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void LoadGitIgnorePatterns_EmptyList_ShouldReturnEmpty()
    {
        // Arrange
        var settings = Options.Create(new ProjectSettings());
        var service = new GitIgnoreService(settings);

        // Act
        var result = service.LoadGitIgnorePatterns(new List<string>(), "C:\\BasePath");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void LoadGitIgnorePatterns_NoExistingFiles_ShouldReturnEmpty()
    {
        // Arrange
        var baseFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(baseFolder);

        var settings = Options.Create(new ProjectSettings());
        var service = new GitIgnoreService(settings);

        // We'll pass paths that do not exist
        var listOfPaths = new List<string>
            {
                "notThere/.gitignore",
                "another/fake.gitignore"
            };

        // Act
        var result = service.LoadGitIgnorePatterns(listOfPaths, baseFolder);

        // Assert
        result.Should().BeEmpty();

        // Cleanup
        Directory.Delete(baseFolder, true);
    }

    [Fact]
    public void LoadGitIgnorePatterns_OneValidGitIgnore_ShouldReturnLines()
    {
        // Arrange
        var baseFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(baseFolder);

        // Create subfolder to match the relative path
        var subFolder = Path.Combine(baseFolder, "sub");
        Directory.CreateDirectory(subFolder);

        // Write a .gitignore
        var gitIgnorePath = Path.Combine(subFolder, ".gitignore");
        File.WriteAllLines(gitIgnorePath, new[]
        {
                "# comment line",
                "",
                "bin/",
                "!bin/special.dll"
            });

        var relativePaths = new List<string> { "sub/.gitignore" };

        var settings = Options.Create(new ProjectSettings());
        var service = new GitIgnoreService(settings);

        // Act
        var result = service.LoadGitIgnorePatterns(relativePaths, baseFolder);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("bin/");
        result.Should().Contain("!bin/special.dll");

        // Cleanup
        Directory.Delete(baseFolder, true);
    }

    [Fact]
    public void LoadGitIgnorePatterns_MultipleGitIgnoreFiles_ShouldCombineAll()
    {
        // Arrange
        var baseFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(baseFolder);

        // File 1
        var sub1 = Path.Combine(baseFolder, "folderA");
        Directory.CreateDirectory(sub1);
        var git1 = Path.Combine(sub1, ".gitignore");
        File.WriteAllLines(git1, new[] { "bin/", "!bin/keep.dll" });

        // File 2
        var sub2 = Path.Combine(baseFolder, "folderB");
        Directory.CreateDirectory(sub2);
        var git2 = Path.Combine(sub2, ".gitignore");
        File.WriteAllLines(git2, new[] { "node_modules/", "dist/" });

        var relativePaths = new List<string>
            {
                "folderA/.gitignore",
                "folderB/.gitignore"
            };

        var settings = Options.Create(new ProjectSettings());
        var service = new GitIgnoreService(settings);

        // Act
        var result = service.LoadGitIgnorePatterns(relativePaths, baseFolder);

        // Assert
        // Should contain 4 lines (no comments/blank in these files)
        result.Should().HaveCount(4);
        result.Should().Contain("bin/");
        result.Should().Contain("!bin/keep.dll");
        result.Should().Contain("node_modules/");
        result.Should().Contain("dist/");

        // Cleanup
        Directory.Delete(baseFolder, true);
    }
}