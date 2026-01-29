using Domain.Common.Exceptions;
using Domain.Common.ValueObjects;
using Domain.SeedWork;

namespace Domain.Entities.Days;

public class Statistics : ValueObject
{
    public DateOnly Date { get; private set; }

    public Rating? UserRating { get; private set; }
    public Rating? LLMRating { get; private set; }

    public Statistics(
            DateOnly date,
            Rating? llmRating = null,
            Rating? userRating = null)
    {
        Date = date;
        LLMRating = llmRating;
        UserRating = userRating;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
        yield return UserRating ?? new object();
        yield return LLMRating ?? new object();
    }



    public void SetUserRating(Rating userRating)
        => UserRating = userRating ?? throw new DomainArgumentNullException(nameof(userRating));

    public void SetLLMRating(Rating llmRating)
        => LLMRating = llmRating ?? throw new DomainArgumentNullException(nameof(llmRating));


    private Statistics() { }
}

