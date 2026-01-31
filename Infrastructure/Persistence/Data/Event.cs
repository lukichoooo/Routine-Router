using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Persistence.Data;

public class Event
{
    [Key]
    public int Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    [Required]
    public required Guid AggregateId { get; set; }

    [Required]
    public required string AggregateIdType { get; set; } = string.Empty;

    [Required]
    public required int Version { get; set; }

    [Required]
    public required string EventType { get; set; } = string.Empty;

    [Required]
    public required string EventData { get; set; } = string.Empty;

    [Required]
    public required DateTimeOffset TimeStamp { get; set; }
}

