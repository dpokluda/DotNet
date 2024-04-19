using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;
using SimpleConsole;

namespace EventHubSender;

/// <summary>
/// An Event Hub sender component. It is responsible for connecting to an Event Hub and then send specified number
/// of messages in batches.
/// </summary>
public class EventHubSender
{
    private readonly EventHubProducerClient _client;
    private readonly ILogger _logger;
    private int _messageNumber = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubSender"/> class.
    /// </summary>
    /// <param name="connectionString">The Event Hub connection string.</param>
    /// <param name="eventHubName">Name of the Event Hub.</param>
    /// <param name="logger">The logger.</param>
    public EventHubSender(string connectionString, string eventHubName, ILogger logger)
    {
        _client = new EventHubProducerClient(connectionString, eventHubName);
        _logger = logger;
    }

    /// <summary>
    /// Sends random messages to the Event Hub asynchronously.
    /// </summary>
    /// <param name="numberOfMessages">Number of messages.</param>
    /// <returns>
    /// Asynchronous task.
    /// </returns>
    public async Task SendMessagesAsync(int numberOfMessages)
    {
        using EventDataBatch eventBatch = await _client.CreateBatchAsync();
        for (int i = 1; i <= numberOfMessages; i++)
        {
            var body = $"{{ \"Version\": 1.0, \"Value\": \"{++_messageNumber}\", \"Timestamp\": \"{DateTime.UtcNow:s}\" }}";
            ConsoleEx.Write(ConsoleColor.Cyan, $"    Sending message: ");
            Console.WriteLine($"{body}");
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(body)));
        }
        
        _logger.LogDebug($"Sending a batch of {numberOfMessages} messages to the event hub.");
        _logger.LogDebug($"Size of the batch is {eventBatch.SizeInBytes} bytes.");
        
        await _client.SendAsync(eventBatch);
    }
}