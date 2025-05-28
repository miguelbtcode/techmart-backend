using Microsoft.Extensions.Logging;
using TechMart.Auth.Application.Common.Models;
using TechMart.Auth.Application.Contracts.Persistence;
using TechMart.Auth.Application.Messaging.Commands;
using TechMart.Auth.Application.Messaging.Pipeline;

namespace TechMart.Auth.Application.Common.Behaviors;

public sealed class TransactionBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = typeof(TRequest).Name;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Transaction started for {RequestName}", requestName);

            var response = await next();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            _logger.LogDebug("Transaction committed successfully for {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}, rolling back", requestName);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

// For commands without response
public sealed class TransactionBehavior<TRequest> : IPipelineBehavior<TRequest, Unit>
    where TRequest : ICommand
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest>> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        TRequest request,
        RequestHandlerDelegate<Unit> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = typeof(TRequest).Name;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Transaction started for {RequestName}", requestName);

            var response = await next();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            _logger.LogDebug("Transaction committed successfully for {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}, rolling back", requestName);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
