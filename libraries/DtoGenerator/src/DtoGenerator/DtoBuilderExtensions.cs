using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DtoGenerator;

public static class DtoBuilderExtensions
{
    extension(SourceProductionContext context)
    {
        // TODO: implement
        public void BuildDtoSourceFiles(GeneratedDtoData dtoData)
        {
            StringBuilder sb = new();
            var entityClassNamespace = dtoData.TargetNamespace;
            var generatedDtoClassName = $"{dtoData.DtoName}.g.cs";

            // Add usings
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");

            // Add target namespace
            sb.AppendLine($"namespace {entityClassNamespace}.Dtos");
            sb.AppendLine("{");

            // Start class
            sb.AppendLine($"\tpublic class {generatedDtoClassName}");
            sb.AppendLine("\t{");

            // for each property in the domain entity, create a corresponding property 
            // in the DTO with the same type
            foreach (var property in dtoData.Properties)
                sb.AppendLine($"\t\t{$"public {property.GetType().FullName} {property.Identifier} {{get; set;}}"}");

            // Add closing braces
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            context.AddSource(generatedDtoClassName, SourceText.From(sb.ToString(), Encoding.UTF8));
            sb.Clear();
        }

    }
}

