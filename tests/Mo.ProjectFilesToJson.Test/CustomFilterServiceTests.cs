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
        var options = Options.Create(new ProjectScannerSettings
        {
            ProjectGitsFileFolder = "SomeTestFolder"
        });
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
        var options = Options.Create(new ProjectScannerSettings
        {
            ProjectGitsFileFolder = "SomeTestFolder"
        });
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
}