using Domain.SeedWork;

namespace Domain.Common.Exceptions
{
    public class DomainArgumentNullException : DomainException
    {
        public DomainArgumentNullException(string message) : base(message)
        {
        }

        public DomainArgumentNullException() : base()
        {
        }

        public DomainArgumentNullException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
