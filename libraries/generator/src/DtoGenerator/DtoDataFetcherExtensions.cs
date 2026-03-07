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

        var mapAttributes = dtoSymbol?.GetAttributes()
            .Where(a => a.AttributeClass?.Name == nameof(MapAttribute) && a is not null)
            ?? [];

        generatorAttr.ExtractGeneratorAttributeConstructorArgs(
                out var targetName,
                out var targetNamespace,
                out var props);

        generatorAttr.ExtractGeneratorAttributeNamedArgs(
                out var include,
                out var exclude);

        if (exclude.Any()) props = props.Where(p => !exclude.Contains(p.Identifier.ValueText));
        if (include.Any()) props = props.Where(p => include.Contains(p.Identifier.ValueText));

        var propNameToMappedType = mapAttributes.GetPropNameToMappedTypeAsDict();

        return new GeneratedDtoData()
        {
            DtoName = dtoSymbol!.Name,
            TargetName = targetName,
            Properties = props,
            TargetNamespace = targetNamespace,
            PropNameToMappedType = propNameToMappedType,
        };
    }

    private static void ExtractGeneratorAttributeConstructorArgs(this AttributeData generatorAttr,
            out string targetName,
            out string targetNamespace,
            out IEnumerable<PropertyDeclarationSyntax> props)
    {
        var targetType = generatorAttr.ConstructorArguments[0].Value as INamedTypeSymbol ?? throw new DtoGeneratorException($"Invalid {nameof(GenerateDtoAttribute)} constructor argument");

        var declarationSyntax = (ClassDeclarationSyntax)targetType.DeclaringSyntaxReferences
            .FirstOrDefault()?.GetSyntax()!;

        props = declarationSyntax.Members.OfType<PropertyDeclarationSyntax>();
        targetNamespace = targetType.ContainingNamespace!.ToDisplayString();
        targetName = targetType.Name;
    }

    private static void ExtractGeneratorAttributeNamedArgs(this AttributeData generatorAttr,
            out ImmutableHashSet<string> include,
            out ImmutableHashSet<string> exclude)
    {
        include = [];
        exclude = [];
        Dictionary<string, TypedConstant> attrArgs = generatorAttr.NamedArguments
            .ToDictionary(x => x.Key, x => x.Value);
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
                $"{nameof(GenerateDtoAttribute)} attribute must have only one of '{nameof(GenerateDtoAttribute.Include)}' or '{nameof(GenerateDtoAttribute.Exclude)}' at the same time");
        }
    }

    private static ImmutableDictionary<string, string> GetPropNameToMappedTypeAsDict(this IEnumerable<AttributeData> mapAttributes)
    {
        Dictionary<string, string> res = [];
        foreach (var mapAttr in mapAttributes)
        {
            var propName = mapAttr.ConstructorArguments[0].Value as string ?? throw new DtoGeneratorException($"Invalid {nameof(MapAttribute.SourcePropName)} constructor argument");
            var mappedType = mapAttr.ConstructorArguments[1].Value as INamedTypeSymbol ?? throw new DtoGeneratorException($"Invalid {nameof(MapAttribute.TargetType)} constructor argument");
            res.Add(propName, mappedType.Name);
        }
        return res.ToImmutableDictionary();
    }

}
