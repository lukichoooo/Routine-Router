namespace EventMapper.SourceGenerators.Common.Exceptions;

public class EventMapperGeneratorException : Exception
{
    public EventMapperGeneratorException() : base()
    {
    }

    public EventMapperGeneratorException(string? message) : base(message)
    {
    }

    public EventMapperGeneratorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

