using System.Text;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using SimpleConsole;

namespace EventHubPartitionReceiver;

/// <summary>
/// An Event Hub partition receiver component. It is responsible for connecting to an Event Hub and then listen for messages
/// in a specific partition.
/// </summary>
public class EventHubPartitionReceiver
{
    private readonly EventHubConsumerClient _client;
    private readonly string _partitionId;
    private readonly BlobClient _blobClient;
    private EventPosition _eventPosition;
    private CancellationTokenSource _cancellationSource;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubPartitionReceiver"/> class.
    /// </summary>
    /// <param name="eventHubConnectionString">The Event Hub connection string.</param>
    /// <param name="eventHubName">Name of the Event Hub.</param>
    /// <param name="consumerGroup">The consumer group name to listen to.</param>
    /// <param name="partitionId">The partition identifier to receive events from.</param>
    /// <param name="storageConnectionString">The storage connection string.</param>
    /// <param name="storageContainer">The blob storage container used for checkpointing.</param>
    /// <param name="logger">The logger.</param>
    public EventHubPartitionReceiver(string eventHubConnectionString, string eventHubName, string consumerGroup, string partitionId, string storageConnectionString, string storageContainer, ILogger logger)
    {
        _logger = logger;
        _client = new EventHubConsumerClient(consumerGroup, eventHubConnectionString, eventHubName);
        _partitionId = partitionId;
        
        var storageClient = new BlobContainerClient(storageConnectionString, storageContainer);
        _blobClient = storageClient.GetBlobClient($"{eventHubName}/{partitionId}");
    }

    /// <summary>
    /// Starts listening to Event Hub partition messages asynchronously.
    /// </summary>
    /// <returns>
    /// Asynchronous task.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    public async Task StartAsync()
    {
        if (_cancellationSource is not null)
        {
            throw new InvalidOperationException("The receiver is already started.");
        }
        
        _eventPosition = await LoadCheckpointAsync();
        _logger.LogDebug($"Loaded checkpoint for partition {_partitionId}");
        _cancellationSource = new CancellationTokenSource();
        
        await OnPartitionInitializingAsync();
        _ = ReceiveEventsAsync(_eventPosition, _cancellationSource.Token);
    }

    /// <summary>
    /// Stops listening to Event Hub messages asynchronously.
    /// </summary>
    /// <returns>
    /// Asynchronous task.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
    public Task StopAsync()
    {
        if (_cancellationSource is null)
        {
            throw new InvalidOperationException("The receiver is not started.");
        }
        
        _cancellationSource.Cancel();
        _cancellationSource = null;
        return Task.CompletedTask;
    }

    private Task OnProcessEventAsync(PartitionEvent partitionEvent)
    {
        var body = Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray());
        ConsoleEx.Write(ConsoleColor.Cyan, $"    Received event on partition {partitionEvent.Partition.PartitionId}: ");
        Console.WriteLine(body);
        return Task.CompletedTask;
    }

    private Task OnProcessErrorAsync(Exception exception)
    {
        ConsoleEx.WriteLine(ConsoleColor.Red, $"Error on partition {_partitionId}: {exception.Message}");
        return Task.CompletedTask;
    }
    
    private Task OnPartitionInitializingAsync()
    {
        ConsoleEx.WriteLine(ConsoleColor.Green, $"Connected to partition {_partitionId}");
        return Task.CompletedTask;
    }
    
    private Task OnPartitionClosingAsync()
    {
        ConsoleEx.WriteLine(ConsoleColor.Magenta, $"Disconnected from partition {_partitionId}");
        return Task.CompletedTask;
    }
    
    private async Task ReceiveEventsAsync(EventPosition eventPosition, CancellationToken cancellationToken)
    {
        try
        {
            await foreach (PartitionEvent partitionEvent in _client.ReadEventsFromPartitionAsync(_partitionId, eventPosition, cancellationToken))
            {
                await OnProcessEventAsync(partitionEvent);

                await SaveCheckpointAsync(partitionEvent.Data.Offset);
                _logger.LogDebug($"Updated checkpoint for partition {_partitionId}");
            }
        }
        catch (TaskCanceledException)
        {
            await OnPartitionClosingAsync();
        }
        catch (Exception e)
        {
            await OnProcessErrorAsync(e);
            await OnPartitionClosingAsync();
        }
    }

    private async Task<EventPosition> LoadCheckpointAsync()
    {
        try
        {
            var response = await _blobClient.DownloadContentAsync();
            var offset = long.Parse(response.Value.Content.ToString());
            return EventPosition.FromOffset(offset, false);
        }
        catch
        {
            // Return null if no checkpoint exists
            return EventPosition.Earliest;
        }
    }

    private async Task SaveCheckpointAsync(long offset)
    {
        var data = Encoding.UTF8.GetBytes(offset.ToString());
        await _blobClient.UploadAsync(new BinaryData(data), overwrite: true);
    }    
}