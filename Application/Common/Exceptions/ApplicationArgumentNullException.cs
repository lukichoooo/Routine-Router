namespace Application.Common.Exceptions;


public class ApplicationArgumentNullException : ApplicationException
{
    public ApplicationArgumentNullException() : base()
    {
    }

    public ApplicationArgumentNullException(string? message) : base(message)
    {
    }

    public ApplicationArgumentNullException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

