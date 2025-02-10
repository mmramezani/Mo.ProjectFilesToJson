using Microsoft.Extensions.DependencyInjection;
using Mo.ProjectFilesToJson.ConsoleApp;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Moq;

namespace Mo.ProjectFilesToJson.Test;

public class ProgramTests
{
    [Fact]
    public void Main_NoExistingSettings_UserEntersNoProjects_ShouldExit()
    {
        // Arrange
        var sp = Helper.ConfigureServices();
        var mockGitIgnoreService = sp.GetService<IGitIgnoreService>() as Mock<IGitIgnoreService>;
        if (mockGitIgnoreService == null)
        {
            // We'll re-mock it manually because the real config doesn't use Moq
        }
    }
}
