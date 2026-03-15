using Microsoft.CodeAnalysis;

namespace EventMapperGenerator.SourceGenerators.ExtensionMethods;

internal static class EventMapperDataFetcherExtensions
{
    extension(Compilation compilation)
    {
        internal IEnumerable<GeneratedEventMapperData> GetAllEvents()
        {
            var baseType = compilation.GetTypeByMetadataName(MappingProfile.BaseEventTypeMetadataName);
            if (baseType is null) return [];

            var referencedAssemblies = compilation.SourceModule.ReferencedAssemblySymbols
                .Concat([compilation.Assembly]);

            return referencedAssemblies
                .SelectMany(assembly => GetAllTypes(assembly.GlobalNamespace))
                .Where(t => IsDerivedFromOrImplements(t, baseType) && !t.IsAbstract)
                .Select(ExtractEventData);
        }

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol namespaceSymbol)
        {
            foreach (var type in namespaceSymbol.GetTypeMembers())
            {
                yield return type;
            }

            foreach (var nestedNamespace in namespaceSymbol.GetNamespaceMembers())
            {
                foreach (var nestedType in GetAllTypes(nestedNamespace))
                {
                    yield return nestedType;
                }
            }
        }

        private static bool IsDerivedFromOrImplements(INamedTypeSymbol type, ITypeSymbol baseType)
        {
            if (SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, baseType))
                return true;

            foreach (var iface in type.AllInterfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(iface.OriginalDefinition, baseType))
                    return true;
            }

            var current = type.BaseType;
            while (current is not null)
            {
                if (SymbolEqualityComparer.Default.Equals(current.OriginalDefinition, baseType))
                    return true;
                current = current.BaseType;
            }

            return false;
        }
    }

    internal static GeneratedEventMapperData ExtractEventData(INamedTypeSymbol eventSymbol)
    {
        var eventNamespace = eventSymbol.ContainingNamespace.ToDisplayString();

        var props = eventSymbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => !p.IsStatic
                    && p.DeclaredAccessibility == Accessibility.Public
                    && !p.IsImplicitlyDeclared
            );

        var payloadProps = props.Where(x =>
                !MappingProfile.IgnoredOnPayloadPropNames.Contains(x.Name));

        var baseProps = props.Where(x =>
                MappingProfile.IgnoredOnPayloadPropNames.Contains(x.Name));

        return new GeneratedEventMapperData()
        {
            EventTypeName = eventSymbol.Name,
            EventNamespace = eventNamespace,
            PayloadProps = payloadProps,
            BaseProps = baseProps,
        };
    }

}

