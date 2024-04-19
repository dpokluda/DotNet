using System.Diagnostics;
using EventHubSender.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleConsole;

namespace EventHubSender;

/// <summary>
/// Main class for the EventHubSender application.
/// </summary>
internal class Program
{
    /// <summary>
    /// Simple tool to send number of events to a configured Event Hub.
    /// </summary>
    /// <param name="number">Number of events to send in a single batch.</param>
    /// <param name="auto">Flag to indicate whether to send events repeatedly.</param>
    /// <param name="delay">Delay in seconds between batches of messages.</param>
    /// <param name="debug">Flag to indicate whether to enable debug logging.</param>
    /// <returns>Exit code for the application. 0 indicates success.</returns>
    static async Task<int> Main(int number, int delay, bool auto, bool debug)
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
            .Configure<EventHubConfig>(configuration.GetSection(EventHubConfig.SectionName))
            .BuildServiceProvider();

        // arguments
        var config = services.GetRequiredService<IOptions<EventHubConfig>>().Value;
        var eventHubName = GetEventHubName(config.ConnectionString);
        Console.WriteLine("Program arguments: ");
        Console.Write("EventHub: ");
        ConsoleEx.WriteLine(ConsoleColor.Yellow, eventHubName);
        Console.Write("Events: ");
        ConsoleEx.WriteLine(ConsoleColor.Yellow, number.ToString());
        Console.Write("Auto  : ");
        ConsoleEx.WriteLine(ConsoleColor.Yellow, auto.ToString());
        Console.Write("Delay : ");
        ConsoleEx.WriteLine(ConsoleColor.Yellow, delay.ToString());
        Console.Write("Debug : ");
        ConsoleEx.WriteLine(ConsoleColor.Yellow, debug.ToString());
        Console.WriteLine();

        // run the program
        var logger = services.GetRequiredService<ILogger<Program>>();
        var sender = new EventHubSender(config.ConnectionString, eventHubName, logger);
        do
        {
            ConsoleEx.WriteLine(ConsoleColor.Yellow, $"Sending batch of {number} messages...");
            await sender.SendMessagesAsync(number);
            
            if (auto)
            {
                Console.WriteLine(Environment.NewLine + $"Waiting for {delay} seconds before sending another batch (press Ctrl+C to cancel/quit)...");
                await Task.Delay(TimeSpan.FromSeconds(delay));
            }
        } while (auto); 

        ConsoleEx.WriteLine(ConsoleColor.Green, "\nFinished.");
        return 0;
    }

    private static string GetEventHubName(string connectionString)
    {
        var endpoint = connectionString.Split(';').FirstOrDefault(s => s.StartsWith("Endpoint"))?.Split('=')[1];
        return new Uri(endpoint).Host.Split('.')[0];
    }
}