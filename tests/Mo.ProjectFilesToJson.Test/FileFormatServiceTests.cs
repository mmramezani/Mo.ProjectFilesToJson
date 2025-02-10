using FluentAssertions;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;
using Mo.ProjectFilesToJson.Core.Services;

namespace Mo.ProjectFilesToJson.Test;

public class FileFormatServiceTests
{
    [Fact]
    public void FormatAsJson_ValidFileList_ShouldReturnJson()
    {
        // Arrange
        IFileFormatService service = new FileFormatService();
        var files = new List<FileContent>
            {
                new FileContent { FilePath = "file1.txt", Content = "Content1" },
                new FileContent { FilePath = "file2.txt", Content = "Content2" }
            };

        // Act
        var result = service.FormatAsJson(files);

        // Assert
        result.Should().Contain("file1.txt");
        result.Should().Contain("Content2");
    }

    [Fact]
    public void FormatWithDivider_ValidFileList_ShouldReturnExpectedString()
    {
        // Arrange
        IFileFormatService service = new FileFormatService();
        var files = new List<FileContent>
            {
                new FileContent { FilePath = "folder/file1.txt", Content = "Content1" },
                new FileContent { FilePath = "folder/file2.txt", Content = "Content2" }
            };

        // Act
        var result = service.FormatWithDivider(files);

        // Assert
        result.Should().Contain("--FILE folder/file1.txt");
        result.Should().Contain("Content2");
        result.Should().Contain("--END (file2.txt)");
    }
}