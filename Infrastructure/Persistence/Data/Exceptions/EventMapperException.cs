namespace Infrastructure.Persistence.Data.Exceptions;


public class EventMapperException : Exception
{
    public EventMapperException(string message, Exception? innerException = null)
        : base(message, innerException) { }

    public EventMapperException(string eventType, string eventData, Exception innerException)
        : base($"Failed to deserialize event of type '{eventType}' with data: {eventData}", innerException) { }

    public EventMapperException() : base()
    {
    }

    public EventMapperException(string? message) : base(message)
    {
    }
}


