namespace Mo.ProjectFilesToJson.ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        var setup = new AppSetup();

        var engine = new AppEngine(setup.ServiceProvider, setup.UserSettingsFile);

        while (true)
        {
            Console.Clear();
            engine.Run();

            Console.WriteLine("Press [Q] to quit, or any other key to start again...");
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Q)
                break;

            Console.WriteLine();
        }

        Console.WriteLine("Goodbye!");
    }
}
