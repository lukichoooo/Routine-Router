using System.Collections.Immutable;
using Attributes.GeneratorAttributes;
using DtoGenerator.SourceGenerators.Common.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DtoGenerator.SourceGenerators;

internal static class DtoDataBuilderExtensions
{
    private const string GeneratorAttribute = "GenerateDto";

    extension(SyntaxNode node)
    {
        public bool HasAttribute()
        {
            if (node is not ClassDeclarationSyntax classDecl)
                return false;

            return classDecl.AttributeLists
                .SelectMany(list => list.Attributes)
                .Any(attr => attr.Name.ToString() == GeneratorAttribute);
        }
    }

    extension(GeneratorSyntaxContext context)
    {
        public GeneratedDtoData ToDtoData()
        {
            var dtoDecl = (ClassDeclarationSyntax)context.Node;
            ISymbol dtoSymbol = context.SemanticModel.GetDeclaredSymbol(dtoDecl)
                ?? throw new DtoGeneratorException($"Symbol not found on '{nameof(GenerateDtoAttribute)}' attribute");

            var generatorAttr = dtoSymbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.Name == nameof(GenerateDtoAttribute))
                ?? throw new DtoGeneratorException($"'{nameof(GenerateDtoAttribute)}' attribute not found");

            var mapAttributes = dtoSymbol.GetAttributes()
                .Where(a => a.AttributeClass?.Name == nameof(MapAttribute) && a is not null)
                ?? [];

            var dtoNamespace = dtoSymbol.ContainingNamespace.ToDisplayString();

            generatorAttr.ExtractGeneratorAttributeConstructorArgs(
                    out var targetName,
                    out var targetNamespace,
                    out var props);

            generatorAttr.ExtractGeneratorAttributeNamedArgs(
                    out var include,
                    out var exclude);

            if (exclude.Any()) props = props.Where(p => !exclude.Contains(p.Name));
            if (include.Any()) props = props.Where(p => include.Contains(p.Name));

            var propNameToMappedType = mapAttributes.GetPropNameToMappedTypeAsDict();

            return new GeneratedDtoData()
            {
                DtoName = dtoSymbol.Name,
                TargetName = targetName,
                Properties = props,
                DtoNamespace = dtoNamespace,
                TargetNamespace = targetNamespace,
                PropNameToMappedType = propNameToMappedType,
            };
        }
    }

    extension(AttributeData generatorAttr)
    {
        private void ExtractGeneratorAttributeConstructorArgs(
                out string targetName,
                out string targetNamespace,
                out IEnumerable<IPropertySymbol> props)
        {
            var targetType = generatorAttr.ConstructorArguments[0].Value as INamedTypeSymbol ?? throw new DtoGeneratorException($"Invalid {nameof(GenerateDtoAttribute)} constructor argument");

            var allProperties = new List<IPropertySymbol>();
            var seenPropertyNames = new HashSet<string>(StringComparer.Ordinal);
            for (var current = targetType; current is not null; current = current.BaseType)
            {
                foreach (var member in current.GetMembers())
                {
                    if (member is IPropertySymbol property
                            && !property.IsStatic
                            && property.ExplicitInterfaceImplementations.IsEmpty
                            && seenPropertyNames.Add(property.Name))
                    {
                        allProperties.Add(property);
                    }
                }
            }
            props = allProperties;
            targetNamespace = targetType.ContainingNamespace.ToDisplayString();
            targetName = targetType.Name;
        }

        private void ExtractGeneratorAttributeNamedArgs(
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
    }


    extension(IEnumerable<AttributeData> mapAttributes)
    {
        private ImmutableDictionary<string, string> GetPropNameToMappedTypeAsDict()
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

}
