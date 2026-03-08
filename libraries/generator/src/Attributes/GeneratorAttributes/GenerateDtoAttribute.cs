using System;

namespace Attributes.GeneratorAttributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class GenerateDtoAttribute(Type TargetType) : Attribute
{
    public Type TargetType { get; } = TargetType;
    public string[] Exclude { get; set; } = [];
    public string[] Include { get; set; } = [];
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class MapAttribute(string sourcePropName, Type targetType) : Attribute
{
    public string SourcePropName { get; } = sourcePropName;
    public Type TargetType { get; } = targetType;
}
