namespace Infrastructure.Persistence.Data.Exceptions
{
    public class EventSerializationException : Exception
    {
        public EventSerializationException(string message, Exception? innerException = null)
            : base(message, innerException) { }

        public EventSerializationException(string eventType, string eventData, Exception innerException)
            : base($"Failed to deserialize event of type '{eventType}' with data: {eventData}", innerException) { }

        public EventSerializationException() : base()
        {
        }

        public EventSerializationException(string? message) : base(message)
        {
        }
    }

}
