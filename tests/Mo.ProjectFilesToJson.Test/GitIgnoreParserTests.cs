using FluentAssertions;
using Mo.ProjectFilesToJson.Core.Services;

namespace Mo.ProjectFilesToJson.Test;

public class GitIgnoreParserTests
{
    [Fact]
    public void IsIgnored_NoPatterns_ShouldReturnFalse()
    {
        // Arrange
        var relativePath = "bin/Debug/file.dll";
        var patterns = new List<string>();

        // Act
        var result = GitIgnoreParser.IsIgnored(relativePath, patterns);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsIgnored_FolderPattern_ShouldReturnTrue()
    {
        // Arrange
        var relativePath = "bin/Debug/file.dll";
        var patterns = new List<string> { "bin/" };

        // Act
        var result = GitIgnoreParser.IsIgnored(relativePath, patterns);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsIgnored_WildcardExtension_ShouldReturnTrue()
    {
        // Arrange
        var relativePath = "myapp.exe";
        var patterns = new List<string> { "*.exe" };

        // Act
        var result = GitIgnoreParser.IsIgnored(relativePath, patterns);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsIgnored_ExactMatch_ShouldReturnTrue()
    {
        // Arrange
        var relativePath = "obj/temp.txt";
        var patterns = new List<string> { "obj/temp.txt" };

        // Act
        var result = GitIgnoreParser.IsIgnored(relativePath, patterns);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsIgnored_WildcardRegex_ShouldReturnTrue()
    {
        // Arrange
        var relativePath = "folder/anyfile.log";
        var patterns = new List<string> { "folder/*.log" };

        // Act
        var result = GitIgnoreParser.IsIgnored(relativePath, patterns);

        // Assert
        result.Should().BeTrue();
    }
}
