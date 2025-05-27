namespace TechMart.Auth.API.Endpoints;

/// <summary>
/// Clase base para endpoints con funcionalidad común
/// </summary>
public abstract class BaseEndpoint : IEndpoint
{
    public abstract void MapEndpoint(IEndpointRouteBuilder app);

    /// <summary>
    /// Obtiene el trace ID del contexto actual
    /// </summary>
    protected static string GetTraceId(HttpContext context) => context.TraceIdentifier;

    /// <summary>
    /// Obtiene la IP del cliente
    /// </summary>
    protected static string? GetClientIp(HttpContext context) =>
        context.Connection.RemoteIpAddress?.ToString();
}
