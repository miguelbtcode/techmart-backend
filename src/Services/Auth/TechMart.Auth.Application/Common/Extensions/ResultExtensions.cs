using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Common.Extensions;

/// <summary>
/// Extension methods for working with Result pattern
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Maps a successful result to a new result with a different value
    /// </summary>
    public static Result<TNew> Map<T, TNew>(this Result<T> result, Func<T, TNew> mapper)
    {
        return result.IsSuccess
            ? Result.Success(mapper(result.Value))
            : Result.Failure<TNew>(result.Error);
    }

    /// <summary>
    /// Maps a successful result to a new result asynchronously
    /// </summary>
    public static async Task<Result<TNew>> MapAsync<T, TNew>(
        this Task<Result<T>> resultTask,
        Func<T, TNew> mapper
    )
    {
        var result = await resultTask;
        return result.Map(mapper);
    }

    /// <summary>
    /// Binds a successful result to a new result-returning function
    /// </summary>
    public static Result<TNew> Bind<T, TNew>(this Result<T> result, Func<T, Result<TNew>> binder)
    {
        return result.IsSuccess ? binder(result.Value) : Result.Failure<TNew>(result.Error);
    }

    /// <summary>
    /// Binds a successful result to a new result-returning function asynchronously
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<T, TNew>(
        this Task<Result<T>> resultTask,
        Func<T, Task<Result<TNew>>> binder
    )
    {
        var result = await resultTask;
        return result.IsSuccess ? await binder(result.Value) : Result.Failure<TNew>(result.Error);
    }

    /// <summary>
    /// Executes an action if the result is successful
    /// </summary>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
            action(result.Value);

        return result;
    }

    /// <summary>
    /// Executes an action if the result is a failure
    /// </summary>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<Error> action)
    {
        if (result.IsFailure)
            action(result.Error);

        return result;
    }

    /// <summary>
    /// Returns the value if successful, otherwise returns the default value
    /// </summary>
    public static T GetValueOrDefault<T>(this Result<T> result, T defaultValue = default!)
    {
        return result.IsSuccess ? result.Value : defaultValue;
    }

    /// <summary>
    /// Converts a Result to a nullable value
    /// </summary>
    public static T? ToNullable<T>(this Result<T> result)
        where T : struct
    {
        return result.IsSuccess ? result.Value : null;
    }

    /// <summary>
    /// Combines multiple results into a single result
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        var failures = results.Where(r => r.IsFailure).ToList();

        if (!failures.Any())
            return Result.Success();

        var errors = failures.Select(f => f.Error).ToArray();
        return Result.Failure(new ValidationError(errors));
    }

    /// <summary>
    /// Combines multiple results with values into a single result
    /// </summary>
    public static Result<IEnumerable<T>> Combine<T>(params Result<T>[] results)
    {
        var failures = results.Where(r => r.IsFailure).ToList();

        if (failures.Any())
        {
            var errors = failures.Select(f => f.Error).ToArray();
            return Result.Failure<IEnumerable<T>>(new ValidationError(errors));
        }

        var values = results.Select(r => r.Value);
        return Result.Success(values);
    }
}
