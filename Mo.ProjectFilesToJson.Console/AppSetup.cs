using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mo.ProjectFilesToJson.Core.Interfaces;
using Mo.ProjectFilesToJson.Core.Models;
using Mo.ProjectFilesToJson.Core.Services;

namespace Mo.ProjectFilesToJson.ConsoleApp;
public class AppSetup
{
    private const string USER_SETTINGS_FILE = "userSettings.json";

    public IConfiguration Configuration { get; }
    public ServiceProvider ServiceProvider { get; }
    public string UserSettingsFile => USER_SETTINGS_FILE;

    public AppSetup()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();
        services.Configure<ProjectSettings>(Configuration.GetSection("ProjectScannerSettings"));

        services.AddSingleton<IGitIgnoreService, GitIgnoreService>();
        services.AddSingleton<IFileScanService, FileScanService>();
        services.AddSingleton<IFileFormatService, FileFormatService>();
        services.AddSingleton<ICustomFilterService, CustomFilterService>();

        ServiceProvider = services.BuildServiceProvider();
    }
}