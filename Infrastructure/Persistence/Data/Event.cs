using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Persistence.Data;

public class Event
{
    [Key]
    public int Id { get; set; }

    [Required]
    public Guid AggregateId { get; set; }

    [Required]
    public string AggregateIdType { get; set; } = string.Empty;

    [Required]
    public int Version { get; set; }

    [Required]
    public string EventType { get; set; } = string.Empty;

    [Required]
    public string EventData { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset TimeStamp { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }
}

