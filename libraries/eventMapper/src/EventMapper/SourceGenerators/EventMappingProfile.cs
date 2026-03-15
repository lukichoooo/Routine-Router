using System.Collections.Immutable;
using EventMapperAbstractions.DbEvents;

namespace EventMapper.SourceGenerators;

public static class EventMappingProfile
{
    public static IImmutableSet<string> GetIgnoredOnPayloadPropNames
        => IDbEvent.IgnoredOnPayloadFields.ToImmutableHashSet();

    public static readonly Type DbEventType = typeof(IDbEvent);

    public static readonly string MapperNamespace = DbEventType.Namespace!;

    public const string MapperClassName = "EventMapper";
}

