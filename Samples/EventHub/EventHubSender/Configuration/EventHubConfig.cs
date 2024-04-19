namespace EventHubSender.Configuration;

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
}