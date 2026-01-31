namespace Infrastructure.Configs;


public sealed record LLMConfig
{
    public required string Uri { get; init; }
    public required string ApiKey { get; init; }
    public required string Model { get; init; }
}

