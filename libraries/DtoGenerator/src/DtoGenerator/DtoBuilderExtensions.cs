using System.Text;
using Microsoft.CodeAnalysis;

namespace DtoGenerator;

public static class DtoBuilderExtensions
{
    extension(SourceProductionContext spc)
    {
        // TODO: implement
        public void BuildDtoSourceFiles(GeneratedDtoData dtoData)
        {
            StringBuilder sb = new();
            sb.AppendLine("class declaration here");

            spc.AddSource($"{dtoData.DtoName}.cs", sb.ToString());
        }

    }
}

