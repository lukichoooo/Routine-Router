namespace Domain.Common;

public static class Clock
{
    public static DateTimeOffset CurrentTime => DateTimeOffset.UtcNow;
}

