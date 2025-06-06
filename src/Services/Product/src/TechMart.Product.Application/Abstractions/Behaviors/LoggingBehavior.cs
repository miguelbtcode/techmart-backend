using MediatR;
using Microsoft.Extensions.Logging;
using TechMart.Product.Application.Abstractions.Messaging;

namespace TechMart.Product.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly ILogger<TRequest> _logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var name = request.GetType().Name;

        try
        {
            var messagePreviewLog = $"Ejecutando el command request {name}";
            _logger.LogInformation(messagePreviewLog);

            var result = await next();

            string messagePostLog = $"El comando {name} se ejecuto exitosamente";
            _logger.LogInformation(messagePostLog);

            return result;
        }
        catch (Exception ex)
        {
            var messageError = $"El comando {name} tuvo errores";
            _logger.LogError(ex, messageError);
            throw;
        }
    }
}