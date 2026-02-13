using Domain.SeedWork;

namespace Domain.Entities.Users.Exceptions;


public class WrongUserCredentialsException : DomainException
{
    public WrongUserCredentialsException(string message) : base(message)
    {
    }

    public WrongUserCredentialsException() : base()
    {
    }

    public WrongUserCredentialsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

