using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DtoGenerator;

public static class DtoDataBuilderExtensions
{
    extension(SyntaxNode syntaxNode)
    {
        public bool HasAttribute(Type attributeType)
            => syntaxNode.GetType().CustomAttributes
                .Any(a => a.AttributeType == attributeType);

    }

    extension(GeneratorSyntaxContext context)
    {
        public GeneratedDtoData ToDtoData(Type attributeType)
        {
            var cds = (ClassDeclarationSyntax)context.Node;
            return new GeneratedDtoData(
                DtoName: cds.Identifier.ToString(),
                ClassName: cds.AttributeLists
                    .Select(attributes => attributes.Attributes
                        .First(a => a.Name.ToString() == attributeType.Name))
                    .First()
                    .ArgumentList!.Arguments[0]!.Expression.ToString(),
                Properties: cds.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .ToHashSet()
            );
        }
    }

}
