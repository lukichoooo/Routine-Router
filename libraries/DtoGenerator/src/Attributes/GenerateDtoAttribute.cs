namespace Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class GenerateDtoAttribute(Type TargetType) : Attribute
{
    public Type TargetType { get; init; } = TargetType;
    public IReadOnlyList<string> Exclude { get; init; } = [];
    public IReadOnlyList<string> Include { get; init; } = [];
}

