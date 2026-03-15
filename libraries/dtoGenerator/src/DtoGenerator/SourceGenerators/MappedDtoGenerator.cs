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
                static (spc, dtoData) => spc.BuildDtoSourceFiles(dtoData)
                    );
    }
}


internal record GeneratedDtoData()
{
    public string DtoName { get; set; } = string.Empty;
    public string TargetName { get; set; } = string.Empty;
    public IEnumerable<IPropertySymbol> Properties { get; set; } = [];
    public string DtoNamespace { get; set; } = string.Empty;
    public string TargetNamespace { get; set; } = string.Empty;
    public IImmutableDictionary<string, string> PropNameToMappedType { get; set; } = ImmutableDictionary<string, string>.Empty;
}

#pragma warning restore RS1041
