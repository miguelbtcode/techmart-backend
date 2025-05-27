using System.Text.Json.Serialization;

namespace TechMart.Auth.API.Common.Responses;

/// <summary>
/// Representa un error en la respuesta de la API
/// </summary>
public sealed record ApiError
{
    [JsonPropertyName("code")]
    public string Code { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonPropertyName("field")]
    public string? Field { get; init; }

    public ApiError(string code, string message, string type = "General", string? field = null)
    {
        Code = code;
        Message = message;
        Type = type;
        Field = field;
    }
}
