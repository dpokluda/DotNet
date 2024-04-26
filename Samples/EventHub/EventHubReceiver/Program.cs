using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleConsole;
using EventHubReceiver.Configuration;

namespace EventHubReceiver;

/// <summary>
/// Main class for the EventHubReceiver application.
/// </summary>
internal class Program
{
    /// <summary>
    /// Simple tool to receive events from a configured Event Hub.
    /// </summary>
    /// <param name="time">Time in seconds to listen for events and then stop.</param>
    /// <param name="debug">Flag to indicate whether to enable debug logging.</param>
    /// <returns>Exit code for the application. 0 indicates success.</returns>
    static async Task<int> Main(int time, bool debug)
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
        var storageName = GetStorageName(config.StorageConnectionString);
        Console.WriteLine("Program arguments: ");
        Console.Write("EventHub : ");
        ConsoleEx.WriteLine(ConsoleColor.Yellow, eventHubName);
        Console.Write("Storage  : ");
        ConsoleEx.WriteLine(ConsoleColor.Yellow, storageName);
        Console.Write("Container: ");
        ConsoleEx.WriteLine(ConsoleColor.Yellow, config.StorageContainerName);
        Console.Write("Debug : ");
        ConsoleEx.WriteLine(ConsoleColor.Yellow, debug.ToString());
        Console.WriteLine();

        // run the program
        var logger = services.GetRequiredService<ILogger<Program>>();
        var receiver = new EventHubReceiver(config.ConnectionString, eventHubName, config.ConsumerGroup, config.StorageConnectionString, config.StorageContainerName, logger);

        ConsoleEx.WriteLine(ConsoleColor.Yellow, "Starting listening for events...");
        await receiver.StartAsync();
        
        if (time > 0)
        {
            ConsoleEx.WriteLine(ConsoleColor.Yellow, $"Listening for {time} seconds...");
            await Task.Delay(TimeSpan.FromSeconds(time));
            ConsoleEx.WriteLine(ConsoleColor.Yellow, "Stopping listening...");
            await receiver.StopAsync();
        }
        else
        {
            ConsoleEx.WriteLine(ConsoleColor.Yellow, "Press any key to stop listening...");
            Console.ReadKey();
            ConsoleEx.WriteLine(ConsoleColor.Yellow, "Stopping listening...");
            await receiver.StopAsync();
        }        

        ConsoleEx.WriteLine(ConsoleColor.Green, "\nFinished.");
        return 0;
    }

    private static string GetEventHubName(string connectionString)
    {
        var endpoint = connectionString.Split(';').FirstOrDefault(s => s.StartsWith("Endpoint"))?.Split('=')[1];
        return new Uri(endpoint).Host.Split('.')[0];
    }
    
    private static string GetStorageName(string connectionString)
    {
        return connectionString.Split(';').FirstOrDefault(s => s.StartsWith("AccountName"))?.Split('=')[1];
    }
}