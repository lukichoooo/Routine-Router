using Domain.SeedWork;

namespace Domain.Common.Exceptions
{
    public class DomainArgumentException : DomainException
    {
        public DomainArgumentException(string message) : base(message)
        {
        }

        public DomainArgumentException() : base()
        {
        }

        public DomainArgumentException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
