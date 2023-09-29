using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        /// - configure program argument parsing
        /// </summary>
        /// <param name="debug">Optional boolean flag to write debug output</param>
        /// <returns>
        /// Exit-code for the process - 0 for success, else an error code.
        /// </returns>
        static async Task<int> Main(bool debug)
        {
            // configure logging and dependency injection
            var services = new ServiceCollection()
                           .AddLogging(configure =>
                           {
                               configure.SetMinimumLevel(debug ? LogLevel.Debug : LogLevel.Information);
                               configure.AddConsole();
                           })
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
            logger.LogInformation("Program is running");
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
            private const string SimpleName = "David Pokluda";
            private readonly ILogger<MyTest> _logger;

            public MyTest(ILogger<MyTest> logger)
            {
                _logger = logger;
            }

            public string Name
            {
                get
                {
                    _logger.LogDebug($"MyTest.Name: {SimpleName}");
                    return SimpleName;
                }
            }
        }
    }
}