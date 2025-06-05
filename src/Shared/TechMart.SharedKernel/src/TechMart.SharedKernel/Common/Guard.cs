using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TechMart.SharedKernel.Common;

/// <summary>
/// Provides guard clauses for validating method arguments and state.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Throws an ArgumentNullException if the argument is null.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="argument">The argument to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The argument if it's not null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the argument is null.</exception>
    public static T NotNull<T>([NotNull] T? argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        where T : class
    {
        if (argument is null)
            throw new ArgumentNullException(parameterName);

        return argument;
    }

    /// <summary>
    /// Throws an ArgumentException if the string argument is null or empty.
    /// </summary>
    /// <param name="argument">The string argument to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The argument if it's not null or empty.</returns>
    /// <exception cref="ArgumentException">Thrown when the argument is null or empty.</exception>
    public static string NotNullOrEmpty([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
    {
        if (string.IsNullOrEmpty(argument))
            throw new ArgumentException("String cannot be null or empty.", parameterName);

        return argument;
    }

    /// <summary>
    /// Throws an ArgumentException if the string argument is null, empty, or whitespace.
    /// </summary>
    /// <param name="argument">The string argument to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The argument if it's not null, empty, or whitespace.</returns>
    /// <exception cref="ArgumentException">Thrown when the argument is null, empty, or whitespace.</exception>
    public static string NotNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException("String cannot be null, empty, or whitespace.", parameterName);

        return argument;
    }

    /// <summary>
    /// Throws an ArgumentException if the collection is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="argument">The collection to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The collection if it's not null or empty.</returns>
    /// <exception cref="ArgumentException">Thrown when the collection is null or empty.</exception>
    public static IEnumerable<T> NotNullOrEmpty<T>([NotNull] IEnumerable<T>? argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
    {
        NotNull(argument, parameterName);

        if (!argument.Any())
            throw new ArgumentException("Collection cannot be empty.", parameterName);

        return argument;
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if the value is not positive.
    /// </summary>
    /// <param name="argument">The value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if it's positive.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not positive.</exception>
    public static int Positive(int argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
    {
        if (argument <= 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value must be positive.");

        return argument;
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if the value is not positive.
    /// </summary>
    /// <param name="argument">The value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if it's positive.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not positive.</exception>
    public static decimal Positive(decimal argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
    {
        if (argument <= 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value must be positive.");

        return argument;
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if the value is negative.
    /// </summary>
    /// <param name="argument">The value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if it's not negative.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is negative.</exception>
    public static int NotNegative(int argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
    {
        if (argument < 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative.");

        return argument;
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if the value is negative.
    /// </summary>
    /// <param name="argument">The value to check.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if it's not negative.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is negative.</exception>
    public static decimal NotNegative(decimal argument, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
    {
        if (argument < 0)
            throw new ArgumentOutOfRangeException(parameterName, "Value cannot be negative.");

        return argument;
    }

    /// <summary>
    /// Throws an ArgumentOutOfRangeException if the value is not within the specified range.
    /// </summary>
    /// <param name="argument">The value to check.</param>
    /// <param name="min">The minimum allowed value (inclusive).</param>
    /// <param name="max">The maximum allowed value (inclusive).</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <returns>The value if it's within the range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not within the range.</exception>
    public static int InRange(int argument, int min, int max, [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
    {
        if (argument < min || argument > max)
            throw new ArgumentOutOfRangeException(parameterName, $"Value must be between {min} and {max}.");

        return argument;
    }

    /// <summary>
    /// Throws an ArgumentException if the condition is false.
    /// </summary>
    /// <param name="condition">The condition to check.</param>
    /// <param name="message">The error message.</param>
    /// <param name="parameterName">The name of the parameter.</param>
    /// <exception cref="ArgumentException">Thrown when the condition is false.</exception>
    public static void Against(bool condition, string message, [CallerArgumentExpression(nameof(condition))] string? parameterName = null)
    {
        if (condition)
            throw new ArgumentException(message, parameterName);
    }

    /// <summary>
    /// Throws an InvalidOperationException if the condition is false.
    /// </summary>
    /// <param name="condition">The condition to check.</param>
    /// <param name="message">The error message.</param>
    /// <exception cref="InvalidOperationException">Thrown when the condition is false.</exception>
    public static void Ensure(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}