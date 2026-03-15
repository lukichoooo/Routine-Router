using System.Collections.Immutable;
using EventMapperAbstractions.DbEvents;
using EventMapperAbstractions.Events;

namespace EventMapper.SourceGenerators;

public static class MappingProfile
{
    public static IImmutableSet<string> GetIgnoredOnPayloadPropNames
        => IDbEvent.IgnoredOnPayloadFields.ToImmutableHashSet();

    public static readonly Type DbEventType = typeof(IDbEvent);
    public static readonly Type BaseEventType = typeof(IEvent<>);

    public static readonly string MapperNamespace = DbEventType.Namespace!;

    public const string MapperClassName = "EventMapper";

    public const string MainFileName = "Main";

    internal static string GetFromDbEventMethodName(string eventTypeName)
        => $"FromDbEventTo{eventTypeName}";

}

