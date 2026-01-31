namespace Infrastructure.Persistence.Data.Exceptions
{
    public class EventSerializerArgumentNullException : EventSerializationException
    {
        public EventSerializerArgumentNullException() : base()
        {
        }

        public EventSerializerArgumentNullException(string? paramName) : base(paramName)
        {
        }

        public EventSerializerArgumentNullException(string eventType, string eventData, Exception innerException) : base(eventType, eventData, innerException)
        {
        }

        public EventSerializerArgumentNullException(string message, Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
