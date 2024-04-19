namespace EventHubReceiver.Configuration;

/// <summary>
/// Event Hub configuration settings.
/// </summary>
public class EventHubConfig
{
    /// <summary>
    /// Section name in the configuration file.
    /// </summary>
    public const string SectionName = "EventHub";

    /// <summary>
    /// Gets or sets the Event Hub connection string.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the group the Event Hub consumer group.
    /// </summary>
    public string ConsumerGroup { get; set; }

    /// <summary>
    /// Gets or sets the storage connection string.
    /// </summary>
    public string StorageConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the name of the blog storage container used for checkpoints.
    /// </summary>
    public string StorageContainerName { get; set; }
}