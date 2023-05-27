internal sealed class DateTimeService : IDateTimeService
{
    private readonly Func<DateTime> _now;

    public DateTime Now => _now();
    public DateOnly Today => DateOnly.FromDateTime(_now());

    public DateTimeService()
    {
        _now = () => DateTime.UtcNow;
    }

    public DateTimeService(Func<DateTime> dateTimeDelegate)
    {
        _now = dateTimeDelegate;
    }
}
