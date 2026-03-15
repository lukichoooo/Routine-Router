using EventMapperGenerator.SourceGenerators.Common.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EventMapperGenerator.SourceGenerators.ExtensionMethods;

internal static class EventMapperDataFetcherExtensions
{
    extension(SyntaxNode node)
    {
        internal bool IsEventCandidate()
            => node is RecordDeclarationSyntax t && t.BaseList is not null;
    }

    extension(GeneratorSyntaxContext context)
    {
        internal GeneratedEventMapperData ToEventData()
        {
            var eventDecl = context.Node;
            var eventSymbol = context.SemanticModel.GetDeclaredSymbol(eventDecl) as INamedTypeSymbol
                ?? throw new EventMapperGeneratorException($"Symbol not found on '{eventDecl.GetType().Name}'");

            var eventInterface = context.SemanticModel.Compilation
                .GetTypeByMetadataName(MappingProfile.BaseEventTypeMetadataName)
                ?? throw new EventMapperGeneratorException($"{MappingProfile.BaseEventTypeMetadataName} not found");

            var eventNamespace = eventSymbol.ContainingNamespace.ToDisplayString();

            var props = eventSymbol?
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => !p.IsStatic
                        && p.DeclaredAccessibility == Accessibility.Public
                        && !p.IsImplicitlyDeclared
                ) ?? [];

            var payloadProps = props.Where(x =>
                    !MappingProfile.IgnoredOnPayloadPropNames.Contains(x.Name));

            var baseProps = props.Where(x =>
                    MappingProfile.IgnoredOnPayloadPropNames.Contains(x.Name));

            return new GeneratedEventMapperData()
            {
                EventTypeName = eventSymbol!.Name,
                EventNamespace = eventNamespace,
                PayloadProps = payloadProps,
                BaseProps = baseProps,
            };
        }
    }
}

