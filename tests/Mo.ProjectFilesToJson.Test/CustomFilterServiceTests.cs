using FluentAssertions;
using Microsoft.Extensions.Options;
using Mo.ProjectFilesToJson.Core.Models;
using Mo.ProjectFilesToJson.Core.Services;

namespace Mo.ProjectFilesToJson.Test;

public class CustomFilterServiceTests
{
    [Fact]
    public void ApplyPathFilters_WithIncludeAndExclude_ShouldFilterCorrectly()
    {
        // Arrange
        var options = Options.Create(new ProjectSettings { });
        var service = new CustomFilterService(options);

        var filePaths = new List<string>
        {
            "folder/a.cs",
            "folder/b.exe",
            "temp/cs/file.txt",
            "app.config",
            "readme.md"
        };

        var includePatterns = new List<string> { "*.cs", "readme.md" };
        var excludePatterns = new List<string> { "*.exe", "app.config" };

        // Act
        var result = service.ApplyPathFilters(filePaths, includePatterns, excludePatterns);

        // Assert
        // We keep only a.cs and readme.md
        result.Should().Contain("folder/a.cs");
        result.Should().Contain("readme.md");
        result.Should().NotContain("folder/b.exe");
        result.Should().NotContain("app.config");
        result.Should().NotContain("temp/cs/file.txt");
    }

    [Fact]
    public void ApplyPathFilters_EmptyIncludeExclude_ShouldReturnAll()
    {
        // Arrange
        var options = Options.Create(new ProjectSettings { });
        var service = new CustomFilterService(options);

        var filePaths = new List<string>
        {
            "folder/a.cs",
            "folder/b.exe"
        };

        // Act
        var result = service.ApplyPathFilters(filePaths, new List<string>(), new List<string>());

        // Assert
        result.Should().Equal(filePaths);
    }

    // ---------------------------------------------------------------------
    // NEW TESTS for ApplyGitIgnoreFilters
    // ---------------------------------------------------------------------

    [Fact]
    public void ApplyGitIgnoreFilters_NoGitIgnorePatterns_ReturnsSameList()
    {
        // Arrange
        var options = Options.Create(new ProjectSettings { });
        var service = new CustomFilterService(options);

        var filePaths = new List<string> { "bin/file1.dll", "src/app.js" };
        var gitIgnorePatterns = new List<string>(); // empty

        // Act
        var result = service.ApplyGitIgnoreFilters(filePaths, gitIgnorePatterns);

        // Assert
        result.Should().BeEquivalentTo(filePaths);
    }

    [Fact]
    public void ApplyGitIgnoreFilters_FolderPattern_ShouldExcludeFiles()
    {
        // Arrange
        var options = Options.Create(new ProjectSettings { });
        var service = new CustomFilterService(options);

        var filePaths = new List<string>
        {
            "bin/file1.dll",
            "bin/sub/file2.dll",
            "src/app.js"
        };
        var gitIgnorePatterns = new List<string> { "bin/" };

        // Act
        var result = service.ApplyGitIgnoreFilters(filePaths, gitIgnorePatterns);

        // Assert
        // "bin/" means anything under bin is ignored
        result.Should().ContainSingle();
        result[0].Should().Be("src/app.js");
    }

    [Fact]
    public void ApplyGitIgnoreFilters_NegativePattern_ShouldUnignoreFiles()
    {
        // Arrange
        var options = Options.Create(new ProjectSettings { });
        var service = new CustomFilterService(options);

        var filePaths = new List<string>
        {
            "bin/file1.dll",
            "bin/SpecialFile.dll",
            "src/app.js"
        };
        var gitIgnorePatterns = new List<string>
        {
            "bin/",
            "!bin/SpecialFile.dll"
        };

        // Act
        var result = service.ApplyGitIgnoreFilters(filePaths, gitIgnorePatterns);

        // Assert
        // bin/ => ignore everything under bin
        // !bin/SpecialFile.dll => un-ignore specifically that file
        result.Should().HaveCount(2);
        result.Should().Contain("bin/SpecialFile.dll");
        result.Should().Contain("src/app.js");
        result.Should().NotContain("bin/file1.dll");
    }

    [Fact]
    public void ApplyGitIgnoreFilters_Wildcard_ShouldExclude()
    {
        // Arrange
        var options = Options.Create(new ProjectSettings { });
        var service = new CustomFilterService(options);

        var filePaths = new List<string>
        {
            "folder/file.log",
            "folder/file.txt",
            "main.exe"
        };
        var gitIgnorePatterns = new List<string>
        {
            "*.log",
            "*.exe"
        };

        // Act
        var result = service.ApplyGitIgnoreFilters(filePaths, gitIgnorePatterns);

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Be("folder/file.txt");
    }
}