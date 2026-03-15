using EventMapper.SourceGenerators.Common.Exceptions;
using EventMapperAbstractions.Events;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EventMapper.SourceGenerators.ExtensionMethods;

public static class EventMapperDataFetcherExtensions
{
    extension(SyntaxNode node)
    {
        internal bool IsEventCandidate()
            => node is TypeDeclarationSyntax t && t.BaseList is not null;
    }

    extension(GeneratorSyntaxContext context)
    {
        internal GeneratedEventMapperData ToEventData()
        {
            var eventDecl = context.Node;
            var eventSymbol = context.SemanticModel.GetDeclaredSymbol(eventDecl) as INamedTypeSymbol
                ?? throw new EventMapperGeneratorException($"Symbol not found on '{eventDecl.GetType().Name}'");

            var eventInterface = context.SemanticModel.Compilation
                .GetTypeByMetadataName(MappingProfile.BaseEventTypeName)
                ?? throw new EventMapperGeneratorException($"{MappingProfile.BaseEventTypeName} not found");

            var eventNamespace = eventSymbol.ContainingNamespace.ToDisplayString();

            var props = eventSymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => !x.IsStatic);

            var payloadProps = props.Where(x =>
                    !MappingProfile.GetIgnoredOnPayloadPropNames.Contains(x.Name));

            var baseProps = props.Where(x =>
                    MappingProfile.GetIgnoredOnPayloadPropNames.Contains(x.Name));

            return new GeneratedEventMapperData()
            {
                EventTypeName = eventSymbol.Name,
                EventNamespace = eventNamespace,
                PayloadProps = payloadProps,
                BaseProps = baseProps,
            };
        }
    }
}

