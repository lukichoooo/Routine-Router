using Attributes;
using DtoGenerator.Common.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DtoGenerator;

public static class DtoDataBuilderExtensions
{
    extension(SyntaxNode node)
    {
        public bool HasAttribute()
        {
            if (node is not ClassDeclarationSyntax classDecl)
                return false;

            return classDecl.AttributeLists
                .SelectMany(list => list.Attributes)
                .Any(attr => attr.Name.ToString() == nameof(GenerateDtoAttribute));
        }
    }

    extension(GeneratorSyntaxContext context)
    {
        public GeneratedDtoData ToDtoData()
        {
            var dtoDecl = (ClassDeclarationSyntax)context.Node;
            var dtoSymbol = context.SemanticModel.GetDeclaredSymbol(dtoDecl);

            var generatorAttr = dtoSymbol?.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == nameof(GenerateDtoAttribute))
                ?? throw new DtoGeneratorException($"'{nameof(GenerateDtoAttribute)}' attribute not found");

            Dictionary<string, TypedConstant> attrArgs = generatorAttr.NamedArguments.ToDictionary();

            string targetName = null!;
            string targetNamespace = null!;
            List<PropertyDeclarationSyntax> props = null!;

            if (attrArgs[nameof(GenerateDtoAttribute.TargetType)].Value is ITypeSymbol targetType)
            {
                var declarationSyntax = (ClassDeclarationSyntax)targetType.DeclaringSyntaxReferences
                    .FirstOrDefault()?.GetSyntax()!;

                props = declarationSyntax.Members.OfType<PropertyDeclarationSyntax>().ToList();
                targetNamespace = targetType.ContainingNamespace?.ToDisplayString()!;
                targetName = targetType.Name;
            }
            if (targetName is null) throw new DtoGeneratorException($"'{nameof(GenerateDtoAttribute.TargetType)}' argument not found");
            if (targetNamespace is null) throw new DtoGeneratorException("Namespace not found for target type");
            if (props is null) throw new DtoGeneratorException($"'{nameof(GenerateDtoAttribute.TargetType)}' argument has invalid type");

            var include = attrArgs[nameof(GenerateDtoAttribute.Include)].Values
                .Select(v => v.Value)
                .ToHashSet();

            var exclude = attrArgs[nameof(GenerateDtoAttribute.Exclude)].Values
                .Select(v => v.Value)
                .ToHashSet();

            props = props
                 .Where(p => (include.Count == 0 || include.Contains(p.Identifier.ValueText))
                         && !exclude.Contains(p.Identifier.ValueText))
                 .ToList();

            return new GeneratedDtoData(
                DtoName: dtoSymbol.Name,
                TargetName: targetName,
                Properties: props,
                TargetNamespace: targetNamespace
            );
        }
    }

}
