using Domain.SeedWork;

namespace Domain.Entities.Days.ValueObjects;

public class Statistics : ValueObject
{
    public DateOnly Date { get; }
    public Rating? UserRating { get; }
    public Rating? LLMRating { get; }

    public Statistics(DateOnly date, Rating? llmRating = null, Rating? userRating = null)
    {
        Date = date;
        UserRating = userRating;
        LLMRating = llmRating;
    }

    public Statistics WithUserRating(Rating userRating)
        => new(Date, LLMRating, userRating);

    public Statistics WithLLMRating(Rating llmRating)
        => new(Date, llmRating, UserRating);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Date;
        yield return UserRating;
        yield return LLMRating;
    }
}


