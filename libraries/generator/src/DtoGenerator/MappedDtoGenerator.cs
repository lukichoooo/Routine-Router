using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DtoGenerator;

// <summary>
//  Creates Converter of Domain Objects to Dtos
//  using source code generation
// </summary>
[Generator(LanguageNames.CSharp)]
public class MappedDtoGenerator : IIncrementalGenerator
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

public struct Mapping
{
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;

    public Mapping(string source, string target) => (Source, Target) = (source, target);
}

public record GeneratedDtoData()
{
    public string DtoName { get; set; } = string.Empty;
    public string TargetName { get; set; } = string.Empty;
    public IEnumerable<PropertyDeclarationSyntax> Properties { get; set; } = [];
    public string TargetNamespace { get; set; } = string.Empty;
    public IEnumerable<Mapping> Maps { get; set; } = [];
}
