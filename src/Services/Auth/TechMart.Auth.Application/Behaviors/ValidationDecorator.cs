using FluentValidation;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Domain.Primitives;

namespace TechMart.Auth.Application.Behaviors;

internal static class ValidationDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        IEnumerable<IValidator<TCommand>> validators
    ) : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(
            TCommand command,
            CancellationToken cancellationToken
        )
        {
            foreach (var validator in validators)
            {
                var validation = await validator.ValidateAsync(command, cancellationToken);
                if (!validation.IsValid)
                    return Result.Failure<TResponse>(validation.Errors.First().ErrorMessage);
            }

            return await inner.Handle(command, cancellationToken);
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> inner,
        IEnumerable<IValidator<TCommand>> validators
    ) : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            foreach (var validator in validators)
            {
                var validation = await validator.ValidateAsync(command, cancellationToken);
                if (!validation.IsValid)
                    return Result.Failure(validation.Errors.First().ErrorMessage);
            }

            return await inner.Handle(command, cancellationToken);
        }
    }
}
