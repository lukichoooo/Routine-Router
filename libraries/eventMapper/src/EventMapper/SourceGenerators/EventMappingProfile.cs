using Domain.SeedWork;
using Infrastructure.Persistence.Data;

namespace EventMapper.SourceGenerators;

public static class EventMappingProfile
{
    internal static IEnumerable<string> GetIgnoredOnPayloadPropNames()
    {
        yield return nameof(IDomainEvent.AggregateId);
        yield return nameof(IDomainEvent.Version);
        yield return nameof(IDomainEvent.Timestamp);
    }

    internal static readonly Type DbEventType = typeof(Event);

    internal static readonly string MapperNamespace = DbEventType.Namespace!;
    internal const string MapperClassName = "EventMapper";

}

