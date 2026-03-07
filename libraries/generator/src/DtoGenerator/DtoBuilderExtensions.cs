using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DtoGenerator;

public static class DtoBuilderExtensions
{
    public static void BuildDtoSourceFiles(
            this SourceProductionContext context,
            GeneratedDtoData dtoData)
    {
        StringBuilder sb = new();

        // Add usings
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Linq;");

        // Add target namespace
        sb.AppendLine($"namespace {dtoData.TargetNamespace}.Dtos");
        sb.AppendLine("{");

        // Start class
        sb.AppendLine($"\tpublic partial class {dtoData.DtoName}");
        sb.AppendLine("\t{");

        // for each property in the domain entity, create a corresponding property 
        // in the DTO with the same type
        foreach (var property in dtoData.Properties)
        {
            string type = property.Type.ToFullString();
            string identifier = property.Identifier.ValueText;

            sb.AppendLine($"\t\t{$"public {type} {identifier} {{get; set;}}"}");
        }

        // Add closing braces
        sb.AppendLine("\t}");
        sb.AppendLine("}");

        context.AddSource(dtoData.DtoName, SourceText.From(sb.ToString(), Encoding.UTF8));
        sb.Clear();
    }

}

