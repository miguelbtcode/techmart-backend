using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    // Propiedades o métodos comunes si los necesitas
}
