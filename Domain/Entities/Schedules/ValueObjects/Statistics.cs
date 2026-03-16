using Domain.SeedWork;

namespace Domain.Entities.Schedules.ValueObjects;

public class Statistics : ValueObject
{
    public Rating? UserRating { get; }
    public Rating? LLMRating { get; }

    public Statistics(Rating? llmRating = null, Rating? userRating = null)
    {
        UserRating = userRating;
        LLMRating = llmRating;
    }

    public Statistics WithUserRating(Rating userRating)
        => new(LLMRating, userRating);

    public Statistics WithLLMRating(Rating llmRating)
        => new(llmRating, UserRating);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return UserRating;
        yield return LLMRating;
    }


    private Statistics() { }
}


