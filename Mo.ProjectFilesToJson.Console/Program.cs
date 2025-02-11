namespace Mo.ProjectFilesToJson.ConsoleApp;

internal class Program
{
    static void Main(string[] args)
    {
        // Build config + DI container
        var setup = new AppSetup();

        // Create our engine that contains the "scan logic"
        var engine = new AppEngine(setup.ServiceProvider, setup.UserSettingsFile);

        while (true)
        {
            Console.Clear();
            // Run one full “scan process”
            engine.Run();

            // Ask if user wants to exit or run again
            Console.WriteLine("Press [Q] to quit, or any other key to start again...");
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Q)
                break;

            // Otherwise loop again
            Console.WriteLine();
        }

        Console.WriteLine("Goodbye!");
    }
}
