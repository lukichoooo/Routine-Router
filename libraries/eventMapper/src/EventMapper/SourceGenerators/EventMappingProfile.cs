using System.Collections.Immutable;
using Infrastructure.Persistence.Data;

namespace EventMapper.SourceGenerators;

public static class EventMappingProfile
{
    public static IImmutableSet<string> GetIgnoredOnPayloadPropNames()
        => Event.IgnoredOnPayloadFields.ToImmutableHashSet();

    public static readonly Type DbEventType = typeof(Event);

    public static readonly string MapperNamespace = DbEventType.Namespace!;

    public const string MapperClassName = "EventMapper";

}

