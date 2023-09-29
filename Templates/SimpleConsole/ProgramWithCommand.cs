using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace SimpleConsole
{
    /// <summary>
    /// Simple program template using full System.CommandLine library.
    /// </summary>
    internal class ProgramWithCommand
    {
        static async Task<int> _Main(string[] args)
        {
            var rootCommand = new RootCommand(@"Simple console application demonstrating how to:
- configure dependency injection
- configure console logging
- configure program argument parsing");
            var debugOption = new Option<bool>("--debug", "Optional boolean flag to write debug output") { IsRequired = false };
            rootCommand.AddOption(debugOption);
            rootCommand.SetHandler(async (debug) =>
                {
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
                },
                debugOption);

            return await rootCommand.InvokeAsync(args);
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