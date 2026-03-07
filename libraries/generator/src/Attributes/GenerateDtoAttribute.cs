using System;
using System.Collections.Generic;

namespace Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class GenerateDtoAttribute(Type TargetType) : Attribute
{
    public Type TargetType { get; set; } = TargetType;
    public IReadOnlyList<string> Exclude { get; set; } = [];
    public IReadOnlyList<string> Include { get; set; } = [];
}

