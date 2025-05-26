using Microsoft.Extensions.Logging;
using Serilog.Context;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Behaviors;

internal static class LoggingDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        ILogger<CommandHandler<TCommand, TResponse>> logger
    ) : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(
            TCommand command,
            CancellationToken cancellationToken
        )
        {
            var name = typeof(TCommand).Name;
            logger.LogInformation("Handling command {Name}", name);

            var result = await inner.Handle(command, cancellationToken);

            if (result.IsFailure)
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                    logger.LogError("Command {Name} failed", name);
            }

            return result;
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> inner,
        ILogger<CommandBaseHandler<TCommand>> logger
    ) : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var name = typeof(TCommand).Name;
            logger.LogInformation("Handling command {Name}", name);

            var result = await inner.Handle(command, cancellationToken);

            if (result.IsFailure)
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                    logger.LogError("Command {Name} failed", name);
            }

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> inner,
        ILogger<QueryHandler<TQuery, TResponse>> logger
    ) : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(
            TQuery query,
            CancellationToken cancellationToken
        )
        {
            var name = typeof(TQuery).Name;
            logger.LogInformation("Handling query {Name}", name);

            var result = await inner.Handle(query, cancellationToken);

            if (result.IsFailure)
            {
                using (LogContext.PushProperty("Error", result.Error, true))
                    logger.LogError("Query {Name} failed", name);
            }

            return result;
        }
    }
}
