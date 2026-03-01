using Attributes;
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
                static (s, _) => s.HasAttribute(typeof(GenerateDtoAttribute)),
                static (s, _) => s.ToDtoData(typeof(GenerateDtoAttribute))
                );

        initContext.RegisterSourceOutput(
                dtoData,
                (spc, dtoData) => spc.BuildDtoSourceFiles(dtoData)
                    );
    }
}

public record GeneratedDtoData(
        string DtoName,
        string ClassName,
        HashSet<PropertyDeclarationSyntax> Properties
        );
