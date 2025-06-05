namespace TechMart.SharedKernel.Common;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// This pattern eliminates the need for throwing exceptions for business logic failures.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error that occurred during the operation, if any.
    /// </summary>
    public Error Error { get; }

    /// <summary>
    /// Gets a value indicating whether the result has an error.
    /// </summary>
    public bool HasError => Error != Error.None;

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must have an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Success() => new(true, Error.None);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed result.</returns>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value of the successful operation.</param>
    /// <returns>A successful result with a value.</returns>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed result.</returns>
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    /// <summary>
    /// Creates a result based on a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="error">The error to use if the condition is false.</param>
    /// <returns>A successful result if the condition is true; otherwise, a failed result.</returns>
    public static Result Create(bool condition, Error error) =>
        condition ? Success() : Failure(error);

    /// <summary>
    /// Creates a result based on a nullable value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The nullable value.</param>
    /// <param name="error">The error to use if the value is null.</param>
    /// <returns>A successful result with the value if not null; otherwise, a failed result.</returns>
    public static Result<TValue> Create<TValue>(TValue? value, Error error) where TValue : class =>
        value is not null ? Success(value) : Failure<TValue>(error);

    /// <summary>
    /// Implicitly converts an Error to a failed Result.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>A failed result.</returns>
    public static implicit operator Result(Error error) => Failure(error);
}

/// <summary>
/// Represents the result of an operation that can either succeed with a value or fail.
/// </summary>
/// <typeparam name="TValue">The type of the value returned on success.</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Gets the value of the successful operation.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessing the value of a failed result.</exception>
    public TValue Value => IsSuccess 
        ? _value! 
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    protected internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value if the result is successful; otherwise, returns the default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the result failed.</param>
    /// <returns>The value if successful; otherwise, the default value.</returns>
    public TValue GetValueOrDefault(TValue defaultValue = default!) =>
        IsSuccess ? _value! : defaultValue;

    /// <summary>
    /// Executes the specified action if the result is successful.
    /// </summary>
    /// <param name="action">The action to execute with the value.</param>
    /// <returns>The current result.</returns>
    public Result<TValue> OnSuccess(Action<TValue> action)
    {
        if (IsSuccess)
            action(_value!);

        return this;
    }

    /// <summary>
    /// Executes the specified action if the result failed.
    /// </summary>
    /// <param name="action">The action to execute with the error.</param>
    /// <returns>The current result.</returns>
    public Result<TValue> OnFailure(Action<Error> action)
    {
        if (IsFailure)
            action(Error);

        return this;
    }

    /// <summary>
    /// Transforms the value if the result is successful.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output value.</typeparam>
    /// <param name="func">The function to transform the value.</param>
    /// <returns>A new result with the transformed value if successful; otherwise, a failed result.</returns>
    public Result<TOutput> Map<TOutput>(Func<TValue, TOutput> func) =>
        IsSuccess ? Result.Success(func(_value!)) : Result.Failure<TOutput>(Error);

    /// <summary>
    /// Binds the result to another operation if successful.
    /// </summary>
    /// <typeparam name="TOutput">The type of the output value.</typeparam>
    /// <param name="func">The function to bind to.</param>
    /// <returns>The result of the bound operation if successful; otherwise, a failed result.</returns>
    public Result<TOutput> Bind<TOutput>(Func<TValue, Result<TOutput>> func) =>
        IsSuccess ? func(_value!) : Result.Failure<TOutput>(Error);

    /// <summary>
    /// Implicitly converts a value to a successful Result.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A successful result with the value.</returns>
    public static implicit operator Result<TValue>(TValue value) => Success(value);

    /// <summary>
    /// Implicitly converts an Error to a failed Result.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>A failed result.</returns>
    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
}