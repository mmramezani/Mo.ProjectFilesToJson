using FluentAssertions;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Services;

namespace Mo.ProjectFilesToJson.Test;

public class FileScanServiceTests
{
    [Fact]
    public void GetAllFilePaths_FolderDoesNotExist_ShouldReturnEmpty()
    {
        // Arrange
        IFileScanService service = new FileScanService();
        var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var ignoreLines = new System.Collections.Generic.List<string>();

        // Act
        var result = service.GetAllFilePaths(folderPath, ignoreLines);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAllFilePaths_NoIgnorePatterns_ShouldReturnAllFiles()
    {
        // Arrange
        IFileScanService service = new FileScanService();
        var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(folderPath);

        var file1 = Path.Combine(folderPath, "file1.txt");
        var file2 = Path.Combine(folderPath, "file2.txt");
        File.WriteAllText(file1, "Content1");
        File.WriteAllText(file2, "Content2");

        // Act
        var result = service.GetAllFilePaths(folderPath, new System.Collections.Generic.List<string>());

        // Assert
        result.Should().HaveCount(2);

        // Cleanup
        Directory.Delete(folderPath, true);
    }

    [Fact]
    public void GetAllFilePaths_IgnorePattern_ShouldExcludeFiles()
    {
        // Arrange
        IFileScanService service = new FileScanService();
        var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(folderPath);

        var binFolder = Path.Combine(folderPath, "bin");
        Directory.CreateDirectory(binFolder);

        var file1 = Path.Combine(folderPath, "file1.cs");
        var file2 = Path.Combine(binFolder, "file2.dll");

        File.WriteAllText(file1, "class File1 {}");
        File.WriteAllText(file2, "binarycontent");

        var ignoreLines = new System.Collections.Generic.List<string> { "bin/" };

        // Act
        var result = service.GetAllFilePaths(folderPath, ignoreLines);

        // Assert
        result.Should().ContainSingle();
        result[0].Should().EndWith("file1.cs");

        // Cleanup
        Directory.Delete(folderPath, true);
    }

    [Fact]
    public void GetAllFilePaths_NegativePattern_ShouldUnignoreFiles()
    {
        // Arrange
        IFileScanService service = new FileScanService();
        var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(folderPath);

        var binFolder = Path.Combine(folderPath, "bin");
        Directory.CreateDirectory(binFolder);

        var file1 = Path.Combine(folderPath, "file1.cs");
        var file2 = Path.Combine(binFolder, "SpecialFile.dll");

        File.WriteAllText(file1, "class File1 {}");
        File.WriteAllText(file2, "binarycontent");

        var ignoreLines = new System.Collections.Generic.List<string>
            {
                "bin/",
                "!bin/SpecialFile.dll"
            };

        // Act
        var result = service.GetAllFilePaths(folderPath, ignoreLines);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.EndsWith("SpecialFile.dll"));

        // Cleanup
        Directory.Delete(folderPath, true);
    }
}