using FluentAssertions;
using Mo.ProjectFilesToJson.Core.Models;

namespace Mo.ProjectFilesToJson.Test;

public class ModelsTests
{
    [Fact]
    public void UserScanSettings_DefaultConstructor_ShouldInitializeFields()
    {
        // Arrange
        var settings = new UserScanSettings();

        // Act & Assert
        settings.ProjectFolderName.Should().BeEmpty();
        settings.SourceFolderPath.Should().BeEmpty();
        settings.DestinationFilePath.Should().BeEmpty();
        settings.FormatIndex.Should().Be(0);
    }

    [Fact]
    public void FileContent_DefaultConstructor_ShouldInitializeFields()
    {
        // Arrange
        var fileContent = new FileContent();

        // Act & Assert
        fileContent.FilePath.Should().BeEmpty();
        fileContent.Content.Should().BeEmpty();
    }

    [Fact]
    public void GitIgnoreEntry_DefaultConstructor_ShouldInitializeFields()
    {
        // Arrange
        var entry = new GitIgnoreEntry();

        // Act & Assert
        entry.IsNegative.Should().BeFalse();
        entry.Pattern.Should().BeEmpty();
    }
}