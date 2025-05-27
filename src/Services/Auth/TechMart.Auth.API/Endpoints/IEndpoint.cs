namespace TechMart.Auth.API.Endpoints;

/// <summary>
/// Interfaz para todos los endpoints
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Mapea el endpoint en el router
    /// </summary>
    void MapEndpoint(IEndpointRouteBuilder app);
}
