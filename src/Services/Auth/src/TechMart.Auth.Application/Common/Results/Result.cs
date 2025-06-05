using System.Text.Json.Serialization;

namespace TechMart.Auth.Application.Common.Results;

public class Result
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public Dictionary<string, string[]>? ValidationErrors { get; private set; }

    [JsonIgnore]
    public bool IsFailure => !IsSuccess;

    protected Result(
        bool isSuccess,
        string message,
        Dictionary<string, string[]>? validationErrors = null
    )
    {
        IsSuccess = isSuccess;
        Message = message;
        ValidationErrors = validationErrors;
    }

    public static Result Success(string message = "Operation completed successfully.")
    {
        return new Result(true, message);
    }

    public static Result Failure(string message)
    {
        return new Result(false, message);
    }

    public static Result Failure(string message, Dictionary<string, string[]> validationErrors)
    {
        return new Result(false, message, validationErrors);
    }
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    private Result(
        bool isSuccess,
        string message,
        T? data = default,
        Dictionary<string, string[]>? validationErrors = null
    )
        : base(isSuccess, message, validationErrors)
    {
        Data = data;
    }

    public static Result<T> Success(T data, string message = "Operation completed successfully.")
    {
        return new Result<T>(true, message, data);
    }

    public static new Result<T> Failure(string message)
    {
        return new Result<T>(false, message);
    }

    public static Result<T> Failure(string message, Dictionary<string, string[]> validationErrors)
    {
        return new Result<T>(false, message, default, validationErrors);
    }
}
