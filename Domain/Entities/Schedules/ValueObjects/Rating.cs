using Domain.SeedWork;

namespace Domain.Entities.Schedules.ValueObjects;

public class Rating : ValueObject // 0 - 100
{
    public byte MoodRating { get; }
    public byte ModivationRating { get; }
    public byte EffortRating { get; }
    public byte ProductivityRating { get; }
    public byte FocusRating { get; }
    public byte StressRating { get; }

    public Rating(
            byte moodRating,
            byte motivationRating,
            byte effortRating,
            byte productivityRating,
            byte focusRating,
            byte stressRating)
    {
        ValidateRatingCriteria(moodRating);
        ValidateRatingCriteria(motivationRating);
        ValidateRatingCriteria(effortRating);
        ValidateRatingCriteria(productivityRating);
        ValidateRatingCriteria(focusRating);
        ValidateRatingCriteria(stressRating);

        MoodRating = moodRating;
        ModivationRating = motivationRating;
        EffortRating = effortRating;
        ProductivityRating = productivityRating;
        FocusRating = focusRating;
        StressRating = stressRating;
    }

    public static void ValidateRatingCriteria(int rating)
    {
        if (rating < 0 || rating > 100)
            throw new DomainException("Rating must be between 0 and 100");
    }


    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return MoodRating;
        yield return ModivationRating;
        yield return EffortRating;
        yield return ProductivityRating;
        yield return FocusRating;
        yield return StressRating;
    }


    public double OverallScore()
    {
        return (MoodRating
                + ModivationRating
                + EffortRating
                + ProductivityRating
                + FocusRating
                + StressRating
                ) / 6.0;
    }


    private Rating() { }
}

