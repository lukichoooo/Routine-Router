using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Attributes;
using DtoGenerator.Common.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DtoGenerator;

public static class DtoDataBuilderExtensions
{
    public static bool HasAttribute(this SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDecl)
            return false;

        return classDecl.AttributeLists
            .SelectMany(list => list.Attributes)
            .Any(attr => attr.Name.ToString() == nameof(GenerateDtoAttribute));
    }

    public static GeneratedDtoData ToDtoData(this GeneratorSyntaxContext context)
    {
        var dtoDecl = (ClassDeclarationSyntax)context.Node;
        var dtoSymbol = context.SemanticModel.GetDeclaredSymbol(dtoDecl);

        var generatorAttr = dtoSymbol?.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == nameof(GenerateDtoAttribute))
            ?? throw new DtoGeneratorException($"'{nameof(GenerateDtoAttribute)}' attribute not found");

        string targetName = null!;
        string targetNamespace = null!;
        List<PropertyDeclarationSyntax> props = null!;

        if (generatorAttr.ConstructorArguments[0].Value is ITypeSymbol targetType)
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


        Dictionary<string, TypedConstant> attrArgs = generatorAttr.NamedArguments
            .ToDictionary(x => x.Key, x => x.Value);

        var include = attrArgs[nameof(GenerateDtoAttribute.Include)].Values
            .Select(v => v.Value)
            .ToImmutableHashSet();

        var exclude = attrArgs[nameof(GenerateDtoAttribute.Exclude)].Values
            .Select(v => v.Value)
            .ToImmutableHashSet();

        if (include.Count != 0 && exclude.Count != 0)
        {
            throw new DtoGeneratorException(
                $"{nameof(GenerateDtoAttribute)} attribute must have at only one of '{nameof(GenerateDtoAttribute.Include)}' or '{nameof(GenerateDtoAttribute.Exclude)}' at the same time");
        }

        if (exclude.Count > 0)
            props = props.Where(p => !exclude.Contains(p.Identifier.ValueText)).ToList();

        if (include.Count > 0)
            props = props.Where(p => include.Contains(p.Identifier.ValueText)).ToList();

        return new GeneratedDtoData()
        {

            DtoName = dtoSymbol.Name,
            TargetName = targetName,
            Properties = props,
            TargetNamespace = targetNamespace,
        };
    }

}
