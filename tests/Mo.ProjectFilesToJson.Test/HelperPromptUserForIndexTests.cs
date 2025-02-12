using FluentAssertions;
using Mo.ProjectFilesToJson.ConsoleApp;
using System.Text;

namespace Mo.ProjectFilesToJson.Test;

public class HelperPromptUserForIndexTests
{
    [Fact]
    public void PromptUserForIndex_InvalidInput_ShouldRepromptUntilValid()
    {

        var consoleInput = new StringBuilder();
        consoleInput.AppendLine("abc");   // invalid
        consoleInput.AppendLine("0");     // invalid (out of range)
        consoleInput.AppendLine("3");     // valid if count=3 => index returned is 2

        using var sr = new StringReader(consoleInput.ToString());
        using var sw = new StringWriter();
        Console.SetIn(sr);
        Console.SetOut(sw);

        var method = typeof(Helper).GetMethod("PromptUserForIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        var result = (int)method.Invoke(null, new object[] { 3 });
        result.Should().Be(2);

        var output = sw.ToString();
        output.Should().Contain("Invalid selection. Try again.");
        output.Should().NotBeEmpty();
    }
}