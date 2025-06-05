namespace TechMart.SharedKernel.Extensions.DateTime;

/// <summary>
/// Extension methods for DateTime.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Gets the start of the day (00:00:00).
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>The start of the day.</returns>
    public static System.DateTime StartOfDay(this System.DateTime dateTime)
    {
        return dateTime.Date;
    }

    /// <summary>
    /// Gets the end of the day (23:59:59.999).
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>The end of the day.</returns>
    public static System.DateTime EndOfDay(this System.DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the week (Monday).
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>The start of the week.</returns>
    public static System.DateTime StartOfWeek(this System.DateTime dateTime)
    {
        var diff = (7 + (dateTime.DayOfWeek - DayOfWeek.Monday)) % 7;
        return dateTime.AddDays(-diff).Date;
    }

    /// <summary>
    /// Gets the end of the week (Sunday).
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>The end of the week.</returns>
    public static System.DateTime EndOfWeek(this System.DateTime dateTime)
    {
        return dateTime.StartOfWeek().AddDays(6).EndOfDay();
    }

    /// <summary>
    /// Gets the start of the month.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>The start of the month.</returns>
    public static System.DateTime StartOfMonth(this System.DateTime dateTime)
    {
        return new System.DateTime(dateTime.Year, dateTime.Month, 1);
    }

    /// <summary>
    /// Gets the end of the month.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>The end of the month.</returns>
    public static System.DateTime EndOfMonth(this System.DateTime dateTime)
    {
        return dateTime.StartOfMonth().AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// Gets the start of the year.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>The start of the year.</returns>
    public static System.DateTime StartOfYear(this System.DateTime dateTime)
    {
        return new System.DateTime(dateTime.Year, 1, 1);
    }

    /// <summary>
    /// Gets the end of the year.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>The end of the year.</returns>
    public static System.DateTime EndOfYear(this System.DateTime dateTime)
    {
        return dateTime.StartOfYear().AddYears(1).AddTicks(-1);
    }

    /// <summary>
    /// Checks if the date is today.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>True if today; otherwise, false.</returns>
    public static bool IsToday(this System.DateTime dateTime)
    {
        return dateTime.Date == System.DateTime.Today;
    }

    /// <summary>
    /// Checks if the date is yesterday.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>True if yesterday; otherwise, false.</returns>
    public static bool IsYesterday(this System.DateTime dateTime)
    {
        return dateTime.Date == System.DateTime.Today.AddDays(-1);
    }

    /// <summary>
    /// Checks if the date is tomorrow.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>True if tomorrow; otherwise, false.</returns>
    public static bool IsTomorrow(this System.DateTime dateTime)
    {
        return dateTime.Date == System.DateTime.Today.AddDays(1);
    }

    /// <summary>
    /// Checks if the date is in the past.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>True if in the past; otherwise, false.</returns>
    public static bool IsInPast(this System.DateTime dateTime)
    {
        return dateTime < System.DateTime.Now;
    }

    /// <summary>
    /// Checks if the date is in the future.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>True if in the future; otherwise, false.</returns>
    public static bool IsInFuture(this System.DateTime dateTime)
    {
        return dateTime > System.DateTime.Now;
    }

    /// <summary>
    /// Checks if the date is a weekend.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>True if weekend; otherwise, false.</returns>
    public static bool IsWeekend(this System.DateTime dateTime)
    {
        return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    /// Checks if the date is a weekday.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>True if weekday; otherwise, false.</returns>
    public static bool IsWeekday(this System.DateTime dateTime)
    {
        return !dateTime.IsWeekend();
    }

    /// <summary>
    /// Gets the age in years from the date.
    /// </summary>
    /// <param name="birthDate">The birth date.</param>
    /// <param name="referenceDate">The reference date (defaults to today).</param>
    /// <returns>The age in years.</returns>
    public static int GetAge(this System.DateTime birthDate, System.DateTime? referenceDate = null)
    {
        var reference = referenceDate ?? System.DateTime.Today;
        var age = reference.Year - birthDate.Year;
        
        if (reference.Month < birthDate.Month || 
            (reference.Month == birthDate.Month && reference.Day < birthDate.Day))
        {
            age--;
        }
        
        return age;
    }

    /// <summary>
    /// Converts to Unix timestamp.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>The Unix timestamp.</returns>
    public static long ToUnixTimestamp(this System.DateTime dateTime)
    {
        return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
    }

    /// <summary>
    /// Converts from Unix timestamp.
    /// </summary>
    /// <param name="unixTimestamp">The Unix timestamp.</param>
    /// <returns>The DateTime.</returns>
    public static System.DateTime FromUnixTimestamp(long unixTimestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
    }

    /// <summary>
    /// Formats the date time as a relative string (e.g., "2 hours ago").
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <param name="referenceDate">The reference date (defaults to now).</param>
    /// <returns>The relative time string.</returns>
    public static string ToRelativeTimeString(this System.DateTime dateTime, System.DateTime? referenceDate = null)
    {
        var reference = referenceDate ?? System.DateTime.Now;
        var timeSpan = reference - dateTime;

        if (timeSpan.TotalDays >= 365)
            return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) == 1 ? "" : "s")} ago";
        
        if (timeSpan.TotalDays >= 30)
            return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) == 1 ? "" : "s")} ago";
        
        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays == 1 ? "" : "s")} ago";
        
        if (timeSpan.TotalHours >= 1)
            return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours == 1 ? "" : "s")} ago";
        
        if (timeSpan.TotalMinutes >= 1)
            return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes == 1 ? "" : "s")} ago";
        
        return "just now";
    }
}