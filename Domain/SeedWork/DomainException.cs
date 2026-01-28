namespace Domain.SeedWork;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {

    }

    public DomainException() : base()
    {
    }

    public DomainException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
