using System.Collections.Immutable;

namespace EventMapper.SourceGenerators;

public static class MappingProfile
{
    public static IImmutableSet<string> GetIgnoredOnPayloadPropNames
        => ImmutableHashSet.Create("AggregateId", "Version", "Timestamp");

    public static readonly string DbEventTypeName = "EventMapperAbstractions.DbEvents.IDbEvent";
    public static readonly string BaseEventTypeName = "EventMapperAbstractions.Events.IEvent`1";

    public static readonly string MapperNamespace = "Generated.EventMapper";

    public const string MapperClassName = "EventMapper";

    public const string MainFileName = "Main";

    internal static string GetFromDbEventMethodName(string eventTypeName)
        => $"FromDbEventTo{eventTypeName}";

}

