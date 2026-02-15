using System.ComponentModel.DataAnnotations;
using Domain.SeedWork;

namespace Infrastructure.Persistence.Data;

public sealed class Event
{
    [Key]
    public int Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }


    // ignored on Payload
    [Required]
    public required Guid AggregateId { get; set; }

    // ignored on Payload
    [Required]
    public required int Version { get; set; }

    // ignored on Payload
    [Required]
    public required DateTimeOffset Timestamp { get; set; }


    // Type data
    [Required]
    public required string AggregateIdType { get; set; }

    // Type data
    [Required]
    public required string EventType { get; set; }


    [Required]
    public required string Payload { get; set; }


    public static Event From(IDomainEvent e, string payload)
        => new()
        {
            AggregateId = e.AggregateId.ToGuid(),
            AggregateIdType = e.AggregateId.GetType().Name,
            EventType = e.GetType().Name,
            Version = e.Version,
            Payload = payload,
            Timestamp = e.Timestamp
        };


    public static IReadOnlySet<string> IgnoredOnPayloadFields
        => new HashSet<string>
        {
            nameof(IDomainEvent.AggregateId),
            nameof(IDomainEvent.Version),
            nameof(IDomainEvent.Timestamp)
        };


    public IEnumerable<(object field, string propName)> GetIgnoredOnPayloadFields()
    {
        yield return (AggregateId, nameof(AggregateId));
        yield return (Version, nameof(Version));
        yield return (Timestamp, nameof(Timestamp));
    }


}

