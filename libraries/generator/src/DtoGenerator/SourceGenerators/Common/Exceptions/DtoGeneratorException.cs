namespace DtoGenerator.SourceGenerators.Common.Exceptions;

internal class DtoGeneratorException : Exception
{
    public DtoGeneratorException() : base() { }
    public DtoGeneratorException(string? message) : base(message) { }
    public DtoGeneratorException(string? message, Exception? innerException) : base(message, innerException) { }
}

