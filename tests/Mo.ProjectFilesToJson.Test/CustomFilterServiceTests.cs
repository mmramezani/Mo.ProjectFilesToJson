using FluentAssertions;
using Mo.ProjectFilesToJson.Core.Services;

namespace Mo.ProjectFilesToJson.Test;

public class CustomFilterServiceTests
{

    [Fact]
    public void ApplyPathFilters_WithIncludeAndExclude_ShouldFilterCorrectly()
    {
        // Arrange
        var service = new CustomFilterService();
        var filePaths = new System.Collections.Generic.List<string>
            {
                "folder/a.cs",
                "folder/b.exe",
                "temp/cs/file.txt",
                "app.config",
                "readme.md"
            };
        var includePatterns = new System.Collections.Generic.List<string> { "*.cs", "readme.md" };
        var excludePatterns = new System.Collections.Generic.List<string> { "*.exe", "app.config" };

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
        var service = new CustomFilterService();
        var filePaths = new System.Collections.Generic.List<string>
            {
                "folder/a.cs",
                "folder/b.exe"
            };

        // Act
        var result = service.ApplyPathFilters(filePaths, new System.Collections.Generic.List<string>(), new System.Collections.Generic.List<string>());

        // Assert
        result.Should().Equal(filePaths);
    }
}
