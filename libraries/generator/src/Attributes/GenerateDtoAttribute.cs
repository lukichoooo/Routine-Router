using System;

namespace Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class GenerateDtoAttribute(Type TargetType) : Attribute
{
    public Type TargetType { get; set; } = TargetType;
    public string[] Exclude { get; set; } = [];
    public string[] Include { get; set; } = [];
}

