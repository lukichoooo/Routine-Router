namespace Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
public class GenerateDtoAttribute : Attribute
{
    public IReadOnlyList<string> Exclude { get; init; } = [];
    public IReadOnlyList<string> Include { get; init; } = [];
}

