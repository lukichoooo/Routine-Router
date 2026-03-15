#pragma warning disable RS1041 // Code analysis requires .net standard 2

using System.Collections.Immutable;
using DtoGenerator.SourceGenerators.ExtensionMethods;
using Microsoft.CodeAnalysis;

namespace DtoGenerator.SourceGenerators;

// <summary>
//  Creates Converter of Domain Objects to Dtos
//  using source code generation
// </summary>
[Generator(LanguageNames.CSharp)]
internal class MappedDtoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        IncrementalValuesProvider<GeneratedDtoData> dtoData = initContext.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => s.HasAttribute(),
                static (ctx, _) => ctx.ToDtoData()
                )
            .Where(x => x is not null);

        initContext.RegisterSourceOutput(
                dtoData,
                static (spc, dtoData) => spc.BuildDtoSourceFile(dtoData)
                    );
    }
}


internal record GeneratedDtoData()
{
    public required string DtoName { get; init; } = string.Empty;
    public required string TargetName { get; init; } = string.Empty;
    public required IEnumerable<IPropertySymbol> Properties { get; init; } = [];
    public required string DtoNamespace { get; init; } = string.Empty;
    public required string TargetNamespace { get; init; } = string.Empty;
    public required IImmutableDictionary<string, string> PropNameToMappedType { get; init; } = ImmutableDictionary<string, string>.Empty;
}

#pragma warning restore RS1041
