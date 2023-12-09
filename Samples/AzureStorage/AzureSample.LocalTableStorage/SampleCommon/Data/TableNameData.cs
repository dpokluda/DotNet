namespace SampleCommon.Data;

public class TableNameData
{
    public string Name { get; init; }
    public List<DateTimeOffset> TimeStamps { get; init; } = new List<DateTimeOffset>();
}