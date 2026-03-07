using System.Collections.Generic;
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
                );

        initContext.RegisterSourceOutput(
                dtoData,
                (spc, dtoData) => spc.BuildDtoSourceFiles(dtoData)
                    );
    }
}

public class GeneratedDtoData
{
    public string DtoName { get; set; }
    public string TargetName { get; set; }
    public IReadOnlyList<PropertyDeclarationSyntax> Properties { get; set; }
    public string TargetNamespace { get; set; }
}
