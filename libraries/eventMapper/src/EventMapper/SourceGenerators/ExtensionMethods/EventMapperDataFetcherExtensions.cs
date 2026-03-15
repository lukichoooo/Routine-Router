using System.Collections.Immutable;
using Domain.SeedWork;
using EventMapper.SourceGenerators.Common.Exceptions;
using Microsoft.CodeAnalysis;

namespace EventMapper.SourceGenerators.ExtensionMethods;

public static class EventMapperDataFetcherExtensions
{
    extension(SyntaxNode node)
    {
        internal bool IsEvent()
            => node is INamedTypeSymbol namedTypeSymbol
                && namedTypeSymbol.AllInterfaces.Any(x => x.Name == nameof(IDomainEvent));
    }

    extension(GeneratorSyntaxContext context)
    {
        internal GeneratedEventMapperData ToEventData()
        {
            var eventDecl = context.Node;
            var eventSymbol = context.SemanticModel.GetDeclaredSymbol(eventDecl) as INamedTypeSymbol
                ?? throw new EventMapperGeneratorException($"Symbol not found on '{eventDecl.GetType().Name}'");

            var eventNamespace = eventSymbol.ContainingNamespace.ToDisplayString();

            var props = eventSymbol.GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => !x.IsStatic);

            var payloadProps = props.Where(x =>
                    !EventMappingProfile.GetIgnoredOnPayloadPropNames.Contains(x.Name));

            var baseProps = props.Where(x =>
                    EventMappingProfile.GetIgnoredOnPayloadPropNames.Contains(x.Name));

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

