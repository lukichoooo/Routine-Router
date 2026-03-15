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
        IncrementalValuesProvider<GeneratedEventMapperData> dtoData = initContext.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => s.IsEventCandidate(),
                static (ctx, _) => ctx.ToEventData()
                )
            .Where(x => x is not null);

        initContext.RegisterSourceOutput(
                dtoData,
                static (spc, dtoData) => spc.BuildMapperSourceFile(dtoData)
                    );

        var dtoDataWrapper = dtoData.Collect();

        initContext.RegisterSourceOutput(
            dtoDataWrapper,
            static (spc, allDtos) =>
            {
                var eventTypeNames = allDtos.Select(d => d.EventTypeName);
                var eventNamespaces = allDtos.Select(d => d.EventNamespace).Distinct();

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
