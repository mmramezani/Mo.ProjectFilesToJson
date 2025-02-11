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
        // 1) Build configuration
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        // 2) Create ServiceCollection and bind config
        var services = new ServiceCollection();
        services.Configure<ProjectScannerSettings>(Configuration.GetSection("ProjectScannerSettings"));

        // 3) Register your services
        services.AddSingleton<IGitIgnoreService, GitIgnoreService>();
        services.AddSingleton<IFileScanService, FileScanService>();
        services.AddSingleton<IFileFormatService, FileFormatService>();
        services.AddSingleton<ICustomFilterService, CustomFilterService>();

        // 4) Build the provider
        ServiceProvider = services.BuildServiceProvider();
    }
}