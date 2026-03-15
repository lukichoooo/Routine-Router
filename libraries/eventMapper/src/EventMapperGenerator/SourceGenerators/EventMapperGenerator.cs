#pragma warning disable RS1041 // Code analysis requires .net standard 2

using EventMapperGenerator.SourceGenerators.ExtensionMethods;
using Microsoft.CodeAnalysis;

namespace EventMapperGenerator.SourceGenerators;

// <summary>
//  Creates Converter of Code C# events and Database Stored Events
//  using source code generation
//  - Event Names must be Unique
// </summary>
[Generator(LanguageNames.CSharp)]
internal class MappedDtoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        var valuesEventsData = initContext.CompilationProvider
            .Select((compilation, _) => compilation.GetAllEvents());

        initContext.RegisterSourceOutput(
                valuesEventsData,
                static (spc, events) =>
                {
                    foreach (var evt in events)
                        spc.BuildMapperSourceFile(evt);

                    var eventTypeNames = events.Select(d => d.EventTypeName);
                    var eventNamespaces = events.Select(d => d.EventNamespace).Distinct();

                    spc.BuildMainSourceFile(eventTypeNames, eventNamespaces);
                }
        );
    }
}


internal record GeneratedEventMapperData()
{
    public required string EventNamespace { get; init; } = string.Empty;
    public required string EventTypeName { get; init; } = string.Empty;
    public required IEnumerable<IPropertySymbol> PayloadProps { get; init; } = [];
    public required IEnumerable<IPropertySymbol> BaseProps { get; init; } = [];
}

#pragma warning restore RS1041
