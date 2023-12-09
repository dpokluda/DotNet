namespace SampleCommon.Data;

public class NameData
{
    public string PartitionId { get; set; } = "names";
    public string id { get; init; }
    public string Name { get; init; }
    public List<DateTimeOffset> TimeStamps { get; set; } = new List<DateTimeOffset>();
}