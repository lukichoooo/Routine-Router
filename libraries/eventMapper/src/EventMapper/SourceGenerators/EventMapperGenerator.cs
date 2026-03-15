#pragma warning disable RS1041 // Code analysis requires .net standard 2

using Microsoft.CodeAnalysis;

namespace EventMapper.SourceGenerators;

// <summary>
//  Creates Converter of Code C# events and Database Stored Events
//  using source code generation
// </summary>
[Generator(LanguageNames.CSharp)]
internal class MappedDtoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
        IncrementalValuesProvider<GeneratedEventData> dtoData = initContext.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => s.IsEvent(),
                static (ctx, _) => ctx.ToEventData()
                )
            .Where(x => x is not null);

        initContext.RegisterSourceOutput(
                dtoData,
                static (spc, dtoData) => spc.BuildMapperSourceFiles(dtoData)
                    );
    }
}


internal record GeneratedEventData()
{
}

#pragma warning restore RS1041
