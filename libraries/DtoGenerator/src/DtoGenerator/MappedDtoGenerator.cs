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
        // define the execution pipeline here via a series of transformations:

        // find all additional files that end with .txt
        IncrementalValuesProvider<AdditionalText> textFiles = initContext.SyntaxProvider
            .CreateSyntaxProvider(
                    static (s, _) => s is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                    static (s, _) => (s as ClassDeclarationSyntax)!.AttributeLists
                    )

        // read their contents and save their name
        IncrementalValuesProvider<(string name, string content)> namesAndContents
            = textFiles.Select(
                    (text, cancellationToken) => (
                        name: Path.GetFileNameWithoutExtension(text.Path),
                        content: text.GetText(cancellationToken)!.ToString()));

        // generate a class that contains their values as const strings
        initContext.RegisterSourceOutput(namesAndContents, (spc, nameAndContent) =>
        {
            spc.AddSource($"ConstStrings.{nameAndContent.name}", $@"
    public static partial class ConstStrings
    {{
        public const string {nameAndContent.name} = ""{nameAndContent.content}"";
    }}");
        });
    }
}
