using System.Collections.Immutable;

namespace EventMapperGenerator.SourceGenerators;

public static class MappingProfile
{
    // HARDCODED
    public static IImmutableSet<string> IgnoredOnPayloadPropNames
        => ImmutableHashSet.Create("AggregateId", "Version", "Timestamp");
    public static IImmutableSet<string> SingleArgConstructorFields
        => ImmutableHashSet.Create("AggregateId");
    public static readonly string DbEventTypeName = "EventMapperAbstractions.DbEvents.IDbEvent";
    public static readonly string BaseEventTypeMetadataName = "EventMapperAbstractions.Events.IEvent`1";
    public static readonly string BaseEventTypeNameFriendly = "EventMapperAbstractions.Events.IEvent";
    // HARDCODED

    public static readonly string MapperNamespace = "Generated.EventMapper";

    public const string MapperClassName = "EventMapper";

    public const string MainFileName = "Main";

    public const string MainFromDbEventMethodName = "FromDbEvent";

    internal static string GetFromDbEventMethodName(string eventTypeName)
        => $"{MainFromDbEventMethodName}To{eventTypeName}";

}

