using Domain.SeedWork;

namespace Domain.Common.ValueObjects;

public class Rating : ValueObject // 0 - 100
{
    public int MoodRating { get; }
    public int ModivationRating { get; }
    public int EffortRating { get; }
    public int ProductivityRating { get; }
    public int FocusRating { get; }
    public int StressRating { get; }

    public Rating(
            int moodRating,
            int motivationRating,
            int effortRating,
            int productivityRating,
            int focusRating,
            int stressRating)
    {
        ValidateRating(moodRating);
        ValidateRating(motivationRating);
        ValidateRating(effortRating);
        ValidateRating(productivityRating);
        ValidateRating(focusRating);
        ValidateRating(stressRating);

        MoodRating = moodRating;
        ModivationRating = motivationRating;
        EffortRating = effortRating;
        ProductivityRating = productivityRating;
        FocusRating = focusRating;
        StressRating = stressRating;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return MoodRating;
        yield return ModivationRating;
        yield return EffortRating;
        yield return ProductivityRating;
        yield return FocusRating;
        yield return StressRating;
    }

    public static void ValidateRating(int rating)
    {
        if (rating < 0 || rating > 100)
            throw new DomainException("Rating must be between 0 and 100");
    }
}

