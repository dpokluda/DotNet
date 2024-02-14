using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleConsole.Configuration;

namespace SimpleConsole
{
    /// <summary>
    /// Simple program template using simplified System.CommandLine.DragonFruit library.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Simple console application demonstrating how to:
        /// - configure dependency injection  
        /// - configure console logging    
        /// - configure application settings with custom configuration class
        /// - configure program argument parsing
        /// </summary>
        /// <param name="debug">Optional boolean flag to write debug output</param>
        /// <returns>
        /// Exit-code for the process - 0 for success, else an error code.
        /// </returns>
        static async Task<int> Main(bool debug)
        {
            // configure application settings
            var configuration = new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                    .Build();

            // configure logging and dependency injection
            var services = new ServiceCollection()
                           .AddLogging(configure =>
                           {
                               configure.SetMinimumLevel(debug ? LogLevel.Debug : LogLevel.Information);
                               configure.AddConsole();
                           })
                           .Configure<MyConfig>(configuration.GetSection(MyConfig.SectionName))
                           .AddSingleton<ITest, MyTest>()
                           .BuildServiceProvider();

            // arguments
            Console.WriteLine("Program arguments: ");
            Console.Write("Debug: ");
            ConsoleEx.WriteLine(ConsoleColor.Yellow, debug.ToString());
            Console.WriteLine();

            // run the program
            await RunAsync(services);

            ConsoleEx.WriteLine(ConsoleColor.Green, "\nFinished.");
            return 0;
        }

        private static Task RunAsync(IServiceProvider services)
        {
            // retrieve logger
            var logger = services.GetRequiredService<ILogger<Program>>();
            string logMessage = "Program is running";
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logMessage += " in debug mode";
            }
            logger.LogInformation(logMessage);
            
            // retrieve singleton dependency
            var test = services.GetRequiredService<ITest>();
            Console.WriteLine($"Name: {test.Name}");

            return Task.CompletedTask;
        }

        private interface ITest
        {
            string Name { get; }
        }

        private class MyTest : ITest
        {
            private const string SimpleName = "Default";
            private readonly MyConfig _myConfig;
            private readonly ILogger<MyTest> _logger;

            public MyTest(IOptions<MyConfig> myConfig, ILogger<MyTest> logger)
            {
                _myConfig = myConfig.Value;
                _logger = logger;
            }

            public string Name
            {
                get
                {
                    var name = _myConfig.Name ?? SimpleName;
                    _logger.LogDebug($"MyTest.Name: {name}");
                    return name;
                }
            }
        }
    }
}