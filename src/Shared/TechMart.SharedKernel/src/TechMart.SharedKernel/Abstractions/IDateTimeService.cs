namespace TechMart.SharedKernel.Abstractions;

/// <summary>
/// Service for providing date and time operations.
/// This abstraction allows for easier testing and consistent time handling across the application.
/// </summary>
public interface IDateTimeService
{
    /// <summary>
    /// Gets the current date and time in UTC.
    /// </summary>
    DateTime UtcNow { get; }
    
    /// <summary>
    /// Gets the current date and time in the local time zone.
    /// </summary>
    DateTime Now { get; }
    
    /// <summary>
    /// Gets the current date (without time) in UTC.
    /// </summary>
    DateOnly UtcToday { get; }
    
    /// <summary>
    /// Gets the current date (without time) in the local time zone.
    /// </summary>
    DateOnly Today { get; }
    
    /// <summary>
    /// Converts a UTC date time to the specified time zone.
    /// </summary>
    /// <param name="utcDateTime">The UTC date time to convert.</param>
    /// <param name="timeZoneId">The time zone identifier (e.g., "America/New_York").</param>
    /// <returns>The date time in the specified time zone.</returns>
    DateTime ConvertFromUtc(DateTime utcDateTime, string timeZoneId);
    
    /// <summary>
    /// Converts a date time from the specified time zone to UTC.
    /// </summary>
    /// <param name="dateTime">The date time to convert.</param>
    /// <param name="timeZoneId">The time zone identifier (e.g., "America/New_York").</param>
    /// <returns>The date time in UTC.</returns>
    DateTime ConvertToUtc(DateTime dateTime, string timeZoneId);
}