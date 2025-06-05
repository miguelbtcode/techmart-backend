using TechMart.SharedKernel.Abstractions;

namespace TechMart.Product.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime Now => DateTime.Now;

    public DateOnly UtcToday => DateOnly.FromDateTime(DateTime.UtcNow);

    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);

    public DateTime ConvertFromUtc(DateTime utcDateTime, string timeZoneId)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
    }

    public DateTime ConvertToUtc(DateTime dateTime, string timeZoneId)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZone);
    }
}