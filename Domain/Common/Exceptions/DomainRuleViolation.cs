using Domain.SeedWork;

namespace Domain.Common.Exceptions;

public class DomainRuleViolation : DomainException
{
    public DomainRuleViolation(string message) : base(message)
    {
    }

    public DomainRuleViolation() : base()
    {
    }

    public DomainRuleViolation(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

