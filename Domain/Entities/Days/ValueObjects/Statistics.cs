using Domain.SeedWork;

namespace Domain.Entities.Days.ValueObjects;

public class Statistics : ValueObject
{
    public DateTimeOffset CreatedAt { get; }
    public Rating? UserRating { get; }
    public Rating? LLMRating { get; }

    public Statistics(DateTimeOffset createdAt, Rating? llmRating = null, Rating? userRating = null)
    {
        CreatedAt = createdAt;
        UserRating = userRating;
        LLMRating = llmRating;
    }

    public Statistics WithUserRating(Rating userRating)
        => new(CreatedAt, LLMRating, userRating);

    public Statistics WithLLMRating(Rating llmRating)
        => new(CreatedAt, llmRating, UserRating);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CreatedAt;
        yield return UserRating;
        yield return LLMRating;
    }
}


