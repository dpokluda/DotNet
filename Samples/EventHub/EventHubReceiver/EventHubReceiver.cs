using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using SimpleConsole;

namespace EventHubReceiver;

/// <summary>
/// An Event Hub receiver component. It is responsible for connecting to an Event Hub and then listen for messages.
/// </summary>
public class EventHubReceiver
{
    private readonly EventProcessorClient _client;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubReceiver"/> class.
    /// </summary>
    /// <param name="eventHubConnectionString">The Event Hub connection string.</param>
    /// <param name="eventHubName">Name of the Event Hub.</param>
    /// <param name="consumerGroup">The consumer group name to listen to.</param>
    /// <param name="storageConnectionString">The storage connection string.</param>
    /// <param name="storageContainer">The blob storage container used for checkpointing.</param>
    /// <param name="logger">The logger.</param>
    public EventHubReceiver(string eventHubConnectionString, string eventHubName, string consumerGroup, string storageConnectionString, string storageContainer, ILogger logger)
    {
        _logger = logger;
        var storageClient = new BlobContainerClient(
            storageConnectionString, storageContainer);
        
        _client = new EventProcessorClient(storageClient, consumerGroup, eventHubConnectionString, eventHubName);
        // partition events
        _client.PartitionInitializingAsync += PartitionInitializingAsync;
        _client.PartitionClosingAsync += PartitionClosingAsync;
        
        // processing events
        _client.ProcessEventAsync += ProcessEventAsync;
        _client.ProcessErrorAsync += ProcessErrorAsync;
    }

    /// <summary>
    /// Starts listening to Event Hub messages asynchronously.
    /// </summary>
    /// <returns>
    /// Asynchronous task.
    /// </returns>
    public async Task StartAsync()
    {
        await _client.StartProcessingAsync();
        _logger.LogDebug("Started processing events.");
    }

    /// <summary>
    /// Stops listening to Event Hub messages asynchronously.
    /// </summary>
    /// <returns>
    /// Asynchronous task.
    /// </returns>
    public async Task StopAsync()
    {
        await _client.StopProcessingAsync();
        _logger.LogDebug("Stopped processing events.");
    }
    
    private async Task ProcessEventAsync(ProcessEventArgs arg)
    {
        var body = Encoding.UTF8.GetString(arg.Data.Body.ToArray());
        ConsoleEx.Write(ConsoleColor.Cyan, $"    Received event on partition {arg.Partition.PartitionId}: ");
        Console.WriteLine(body);
        await arg.UpdateCheckpointAsync();
        _logger.LogDebug($"Updated checkpoint for partition {arg.Partition.PartitionId}");
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        ConsoleEx.WriteLine(ConsoleColor.Red, $"Error on partition {arg.PartitionId}: {arg.Exception.Message}");
        return Task.CompletedTask;
    }

    private Task PartitionInitializingAsync(PartitionInitializingEventArgs arg)
    {
        ConsoleEx.WriteLine(ConsoleColor.Green, $"Connected to partition {arg.PartitionId}");
        return Task.CompletedTask;
    }
    
    private Task PartitionClosingAsync(PartitionClosingEventArgs arg)
    {
        ConsoleEx.WriteLine(ConsoleColor.Magenta, $"Disconnected from partition {arg.PartitionId}");
        return Task.CompletedTask;
    }
}