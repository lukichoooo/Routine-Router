namespace Domain.Common
{
    public static class Clock
    {
        public static DateTimeOffset Now => DateTimeOffset.UtcNow;
    }
}
