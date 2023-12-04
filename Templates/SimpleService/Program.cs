using SimpleService.Configuration;

namespace SimpleService
{
    public class Program
    {
        /// <summary>
        /// Simple background service demonstrating how to:
        /// - configure dependency injection  
        /// - configure console logging  
        /// - configure program argument parsing
        /// </summary>
        /// <param name="debug">Optional boolean flag to write debug output</param>
        /// <param name="interval">Optional interval in seconds to run</param>
        /// <returns>
        /// Exit-code for the process - 0 for success, else an error code.
        /// </returns>
        public static void Main(bool debug, int interval = 1)
        {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddLogging(configure =>
                    {
                        configure.SetMinimumLevel(debug ? LogLevel.Debug : LogLevel.Information);
                        configure.AddConsole();
                    });
                    // register MyConfig configuration section
                    services.AddOptions<MyConfig>()
                            .BindConfiguration(MyConfig.SectionName);
                    services.AddSingleton<ITest, MyTest>();
                    services.AddSingleton<IProgramArguments>(new ProgramArguments()
                    {
                        Debug = debug, 
                        IntervalInSeconds = interval
                    });
                    services.AddHostedService<Worker>();
                })
                .Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            // arguments
            var arguments = host.Services.GetRequiredService<IProgramArguments>();
            Console.WriteLine("Program arguments: ");
            Console.Write("Debug   : ");
            ConsoleEx.WriteLine(ConsoleColor.Yellow, arguments.Debug.ToString());
            logger.LogInformation("Program arguments: debug={debug}", arguments.Debug);
            Console.Write("Interval: ");
            ConsoleEx.Write(ConsoleColor.Yellow, arguments.IntervalInSeconds.ToString());
            ConsoleEx.WriteLine(" second");
            logger.LogInformation("Program arguments: intervalInSeconds={interval}", arguments.IntervalInSeconds);
            Console.WriteLine();

            ConsoleEx.WriteLine(ConsoleColor.Yellow, "Starting the background worker...");
            logger.LogDebug("Starting the background worker...");
            host.Run();

            ConsoleEx.WriteLine(ConsoleColor.Red, "\nStopped.");
        }
    }

    public interface IProgramArguments
    {
        bool Debug { get; }
        int IntervalInSeconds { get; }
    }

    public class ProgramArguments: IProgramArguments
    {
        public bool Debug { get; set; }
        public int IntervalInSeconds { get; set; }
    }
}