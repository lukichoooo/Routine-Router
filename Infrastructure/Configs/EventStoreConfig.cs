namespace Infrastructure.Configs;

public class EventStoreConfig
{
    public const string SectionName = "EventStore";
    public string ConnectionString { get; set; } = "Data Source=events.db";
    public bool CreateDatabaseIfNotExists { get; set; } = true;
}
