using System.Collections.Immutable;
using Attributes;
using DtoGenerator.Common.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DtoGenerator;

public static class DtoDataBuilderExtensions
{
    private const string GeneratorAttribute = "GenerateDto";

    public static bool HasAttribute(this SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDecl)
            return false;

        return classDecl.AttributeLists
            .SelectMany(list => list.Attributes)
            .Any(attr => attr.Name.ToString() == GeneratorAttribute);
    }

    public static GeneratedDtoData ToDtoData(this GeneratorSyntaxContext context)
    {
        var dtoDecl = (ClassDeclarationSyntax)context.Node;
        var dtoSymbol = context.SemanticModel.GetDeclaredSymbol(dtoDecl);

        var generatorAttr = dtoSymbol?.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == nameof(GenerateDtoAttribute))
            ?? throw new DtoGeneratorException($"'{nameof(GenerateDtoAttribute)}' attribute not found");

        string targetName = string.Empty;
        string targetNamespace = string.Empty;
        IEnumerable<PropertyDeclarationSyntax> props = [];

        if (generatorAttr.ConstructorArguments[0].Value is ITypeSymbol targetType)
        {
            var declarationSyntax = (ClassDeclarationSyntax)targetType.DeclaringSyntaxReferences
                .FirstOrDefault()?.GetSyntax()!;

            props = declarationSyntax.Members.OfType<PropertyDeclarationSyntax>();
            targetNamespace = targetType.ContainingNamespace!.ToDisplayString();
            targetName = targetType.Name;
        }
        if (string.IsNullOrEmpty(targetName)) throw new DtoGeneratorException($"'{nameof(GenerateDtoAttribute.TargetType)}' argument not found");
        if (string.IsNullOrEmpty(targetNamespace)) throw new DtoGeneratorException("Namespace not found for target type");
        if (!props.Any()) throw new DtoGeneratorException($"'{nameof(GenerateDtoAttribute.TargetType)}' argument has invalid type");


        Dictionary<string, TypedConstant> attrArgs = generatorAttr.NamedArguments
            .ToDictionary(x => x.Key, x => x.Value);

        ImmutableHashSet<string> include = [];
        ImmutableHashSet<string> exclude = [];

        if (attrArgs.TryGetValue(nameof(GenerateDtoAttribute.Include), out var includeArg))
        {
            include = includeArg.Values
                .Select(v => v.Value as string)
                .ToImmutableHashSet() as ImmutableHashSet<string>;
        }

        if (attrArgs.TryGetValue(nameof(GenerateDtoAttribute.Exclude), out var excludeArg))
        {
            exclude = excludeArg.Values
                .Select(x => x.Value as string)
                .ToImmutableHashSet() as ImmutableHashSet<string>;
        }

        if (include.Count != 0 && exclude.Count != 0)
        {
            throw new DtoGeneratorException(
                $"{nameof(GenerateDtoAttribute)} attribute must have at only one of '{nameof(GenerateDtoAttribute.Include)}' or '{nameof(GenerateDtoAttribute.Exclude)}' at the same time");
        }

        if (exclude.Count > 0)
            props = props.Where(p => !exclude.Contains(p.Identifier.ValueText));

        if (include.Count > 0)
            props = props.Where(p => include.Contains(p.Identifier.ValueText));

        return new GeneratedDtoData()
        {
            DtoName = dtoSymbol.Name,
            TargetName = targetName,
            Properties = props,
            TargetNamespace = targetNamespace,
            Maps = [],
        };
    }

}
