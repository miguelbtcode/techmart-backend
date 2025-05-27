namespace TechMart.Auth.API.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public List<ErrorItem>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Successfull(T data, string message = "Success") =>
        new()
        {
            Success = true,
            Message = message,
            Data = data,
        };

    public static ApiResponse<T> Failure(string message, List<ErrorItem> errors) =>
        new()
        {
            Success = false,
            Message = message,
            Errors = errors,
        };
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public List<ErrorItem>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse Failure(string message, List<ErrorItem> errors) =>
        new()
        {
            Success = false,
            Message = message,
            Errors = errors,
        };

    public static ApiResponse Successfull(string message = "Success") =>
        new() { Success = true, Message = message };
}

public class ErrorItem
{
    public string Type { get; set; } = string.Empty; // e.g., "Validation", "Authorization"
    public string Code { get; set; } = string.Empty; // e.g., "EMAIL_REQUIRED"
    public string Message { get; set; } = string.Empty; // mensaje legible
}
