using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Mo.ProjectFilesToJson.ConsoleApp;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;

namespace Mo.ProjectFilesToJson.Test;

public class HelperTests
{
    [Fact]
    public void LoadUserSettings_FileDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var filePath = "NonExistentFile.json";

        // Act
        var result = Helper.LoadUserSettings(filePath);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void LoadUserSettings_FileExistsButInvalidJson_ShouldReturnNull()
    {
        // Arrange
        var filePath = Path.GetTempFileName();
        File.WriteAllText(filePath, "Invalid JSON");

        // Act
        var result = Helper.LoadUserSettings(filePath);

        // Assert
        result.Should().BeNull();

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void LoadUserSettings_ValidJsonMissingFields_ShouldReturnNull()
    {
        // Arrange
        var filePath = Path.GetTempFileName();
        var invalidSettings = new { SomeOtherField = "value" };
        File.WriteAllText(filePath, JsonConvert.SerializeObject(invalidSettings));

        // Act
        var result = Helper.LoadUserSettings(filePath);

        // Assert
        result.Should().BeNull();

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void LoadUserSettings_ValidJsonWithEmptyFields_ShouldReturnNull()
    {
        // Arrange
        var filePath = Path.GetTempFileName();
        var invalidSettings = new UserScanSettings
        {
            ProjectFolderName = "",
            SourceFolderPath = "",
            DestinationFilePath = ""
        };
        File.WriteAllText(filePath, JsonConvert.SerializeObject(invalidSettings));

        // Act
        var result = Helper.LoadUserSettings(filePath);

        // Assert
        result.Should().BeNull();

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void LoadUserSettings_ValidJson_ShouldReturnUserScanSettings()
    {
        // Arrange
        var filePath = Path.GetTempFileName();
        var validSettings = new UserScanSettings
        {
            ProjectFolderName = "TestProject",
            SourceFolderPath = "C:\\Test\\Source",
            DestinationFilePath = "C:\\Test\\Destination",
            FormatIndex = 1
        };
        File.WriteAllText(filePath, JsonConvert.SerializeObject(validSettings));

        // Act
        var result = Helper.LoadUserSettings(filePath);

        // Assert
        result.Should().NotBeNull();
        result.ProjectFolderName.Should().Be("TestProject");
        result.SourceFolderPath.Should().Be("C:\\Test\\Source");
        result.DestinationFilePath.Should().Be("C:\\Test\\Destination");
        result.FormatIndex.Should().Be(1);

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void SaveUserSettings_ValidSettings_ShouldWriteFile()
    {
        // Arrange
        var filePath = Path.GetTempFileName();
        var settings = new UserScanSettings
        {
            ProjectFolderName = "TestProject",
            SourceFolderPath = "C:\\Test\\Source",
            DestinationFilePath = "C:\\Test\\Destination",
            FormatIndex = 0
        };

        // Act
        Helper.SaveUserSettings(settings, filePath);

        // Assert
        var savedContent = File.ReadAllText(filePath);
        savedContent.Should().Contain("TestProject");

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void PrintUserSettings_ValidSettings_ShouldPrintToConsole()
    {
        // Arrange
        var settings = new UserScanSettings
        {
            ProjectFolderName = "TestProject",
            SourceFolderPath = "C:\\Test\\Source",
            DestinationFilePath = "C:\\Test\\Destination",
            FormatIndex = 0
        };
        using var sw = new StringWriter();
        Console.SetOut(sw);

        // Act
        Helper.PrintUserSettings(settings);

        // Assert
        var output = sw.ToString();
        output.Should().Contain("TestProject");
        output.Should().Contain("C:\\Test\\Source");
        output.Should().Contain("C:\\Test\\Destination");
    }

    [Fact]
    public void ReadFileContents_ValidFiles_ShouldReturnContent()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var filePath1 = Path.Combine(tempDir, "file1.txt");
        var filePath2 = Path.Combine(tempDir, "file2.txt");
        File.WriteAllText(filePath1, "Content1");
        File.WriteAllText(filePath2, "Content2");

        var filteredPaths = new System.Collections.Generic.List<string> { "file1.txt", "file2.txt" };

        // Act
        var result = Helper.ReadFileContents(tempDir, filteredPaths);

        // Assert
        result.Should().HaveCount(2);
        result[0].Content.Should().Be("Content1");
        result[1].Content.Should().Be("Content2");

        // Cleanup
        Directory.Delete(tempDir, true);
    }

    [Fact]
    public void ReadFileContents_SomeFilesMissing_ShouldOnlyReturnExistingOnes()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        var filePath1 = Path.Combine(tempDir, "file1.txt");
        File.WriteAllText(filePath1, "Content1");

        var filteredPaths = new System.Collections.Generic.List<string> { "file1.txt", "file2.txt" };

        // Act
        var result = Helper.ReadFileContents(tempDir, filteredPaths);

        // Assert
        result.Should().HaveCount(1);
        result[0].Content.Should().Be("Content1");

        // Cleanup
        Directory.Delete(tempDir, true);
    }

    [Fact]
    public void BuildOutputFilePath_EmptyInput_ShouldReturnResultTxtInCurrentDirectory()
    {
        // Arrange
        var input = string.Empty;
        var currentDir = Directory.GetCurrentDirectory();
        var expected = Path.Combine(currentDir, "Result.txt");

        // Act
        var result = Helper.BuildOutputFilePath(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void BuildOutputFilePath_InputWithExtension_ShouldReturnFullPathAsIs()
    {
        // Arrange
        var input = "C:\\Folder\\custom.txt";
        var expected = Path.GetFullPath(input);

        // Act
        var result = Helper.BuildOutputFilePath(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void BuildOutputFilePath_InputWithoutExtension_ShouldAppendResultTxt()
    {
        // Arrange
        var input = "C:\\Folder\\SubFolder";
        var expected = Path.Combine(Path.GetFullPath(input), "Result.txt");

        // Act
        var result = Helper.BuildOutputFilePath(input);

        // Assert
        result.Should().Be(expected);
    }
}