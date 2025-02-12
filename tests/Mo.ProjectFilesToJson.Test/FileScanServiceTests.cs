using FluentAssertions;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Services;

namespace Mo.ProjectFilesToJson.Test;

public class FileScanServiceTests
{
    [Fact]
    public void GetAllFilePaths_FolderDoesNotExist_ShouldReturnEmptyLists()
    {
        // Arrange
        IFileScanService service = new FileScanService();
        var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act
        var (allFiles, gitIgnoreFiles) = service.GetAllFilePaths(folderPath);

        // Assert
        allFiles.Should().BeEmpty();
        gitIgnoreFiles.Should().BeEmpty();
    }

    [Fact]
    public void GetAllFilePaths_FolderExists_NoGitIgnore_ShouldReturnAllRegularFiles()
    {
        // Arrange
        IFileScanService service = new FileScanService();
        var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(folderPath);

        var file1 = Path.Combine(folderPath, "file1.txt");
        var file2 = Path.Combine(folderPath, "file2.cs");
        File.WriteAllText(file1, "Content1");
        File.WriteAllText(file2, "Content2");

        // Act
        var (allFiles, gitIgnoreFiles) = service.GetAllFilePaths(folderPath);

        // Assert
        allFiles.Should().HaveCount(2);
        gitIgnoreFiles.Should().BeEmpty();

        allFiles.Should().Contain(x => x.EndsWith("file1.txt"));
        allFiles.Should().Contain(x => x.EndsWith("file2.cs"));

        // Cleanup
        Directory.Delete(folderPath, true);
    }

    [Fact]
    public void GetAllFilePaths_FolderExists_WithGitIgnore_ShouldSeparateThem()
    {
        // Arrange
        IFileScanService service = new FileScanService();
        var folderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(folderPath);

        var file1 = Path.Combine(folderPath, "file1.txt");
        var gitIgnore = Path.Combine(folderPath, ".gitignore");
        File.WriteAllText(file1, "Some content");
        File.WriteAllText(gitIgnore, "# some ignore rules");

        // Act
        var (allFiles, gitIgnoreFiles) = service.GetAllFilePaths(folderPath);

        // Assert
        allFiles.Should().HaveCount(1);
        allFiles[0].Should().EndWith("file1.txt");

        gitIgnoreFiles.Should().HaveCount(1);
        gitIgnoreFiles[0].Should().EndWith(".gitignore");

        // Cleanup
        Directory.Delete(folderPath, true);
    }

    [Fact]
    public void GetAllFilePaths_MultipleSubfoldersWithGitIgnore_ShouldFindAll()
    {
        // Arrange
        IFileScanService service = new FileScanService();
        var baseDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(baseDir);

        // Create some regular files
        File.WriteAllText(Path.Combine(baseDir, "root.txt"), "root content");

        // Create subfolder
        var subDir = Path.Combine(baseDir, "SubFolder");
        Directory.CreateDirectory(subDir);
        File.WriteAllText(Path.Combine(subDir, "sub.txt"), "sub content");

        // Create .gitignore in root
        File.WriteAllText(Path.Combine(baseDir, ".gitignore"), "node_modules/");

        // Create another .gitignore in the subfolder
        File.WriteAllText(Path.Combine(subDir, ".gitignore"), "*.log");

        // Act
        var (allFiles, gitIgnoreFiles) = service.GetAllFilePaths(baseDir);

        // Assert
        allFiles.Should().HaveCount(2);      // root.txt, sub.txt
        gitIgnoreFiles.Should().HaveCount(2); // one in root, one in SubFolder

        // Cleanup
        Directory.Delete(baseDir, true);
    }
}
