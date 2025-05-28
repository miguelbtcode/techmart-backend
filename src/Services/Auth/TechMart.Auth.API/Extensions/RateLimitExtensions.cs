using TechMart.Auth.API.Middleware;
using TechMart.Auth.Application.Common.Decorators;
using TechMart.Auth.Application.Contracts.Infrastructure;
using TechMart.Auth.Application.Features.Authentication.Commands.Login;
using TechMart.Auth.Application.Features.Users.Commands.CreateUser;
using TechMart.Auth.Application.Features.Users.Commands.ForgotPassword;
using TechMart.Auth.Application.Messaging.Commands;

namespace TechMart.Auth.API.Extensions;

public static class RateLimitExtensions
{
    /// <summary>
    /// Adds rate limiting services and configuration
    /// </summary>
    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Configure rate limiting options
        services.Configure<RateLimitOptions>(
            configuration.GetSection(RateLimitOptions.SectionName)
        );

        // Register rate limiting strategies
        services.AddScoped<IRateLimitStrategy<LoginCommand>, LoginCommandRateLimitStrategy>();
        services.AddScoped<
            IRateLimitStrategy<CreateUserCommand>,
            CreateUserCommandRateLimitStrategy
        >();
        services.AddScoped<
            IRateLimitStrategy<ForgotPasswordCommand>,
            ForgotPasswordCommandRateLimitStrategy
        >();

        // Register decorators
        services.AddRateLimitingDecorators();

        return services;
    }

    /// <summary>
    /// Registers rate limiting decorators for specific commands
    /// </summary>
    private static IServiceCollection AddRateLimitingDecorators(this IServiceCollection services)
    {
        // Decorate login command handler
        services.Decorate<ICommandHandler<LoginCommand, LoginCommandVm>>(
            (handler, serviceProvider) =>
                new RateLimitCommandDecorator<LoginCommand, LoginCommandVm>(
                    handler,
                    serviceProvider.GetRequiredService<IRateLimitService>(),
                    serviceProvider.GetRequiredService<IRateLimitStrategy<LoginCommand>>(),
                    serviceProvider.GetRequiredService<
                        ILogger<RateLimitCommandDecorator<LoginCommand, LoginCommandVm>>
                    >()
                )
        );

        // Decorate registration command handler
        services.Decorate<ICommandHandler<CreateUserCommand, CreateUserCommandVm>>(
            (handler, serviceProvider) =>
                new RateLimitCommandDecorator<CreateUserCommand, CreateUserCommandVm>(
                    handler,
                    serviceProvider.GetRequiredService<IRateLimitService>(),
                    serviceProvider.GetRequiredService<IRateLimitStrategy<CreateUserCommand>>(),
                    serviceProvider.GetRequiredService<
                        ILogger<RateLimitCommandDecorator<CreateUserCommand, CreateUserCommandVm>>
                    >()
                )
        );

        // Decorate forgot password command handler
        services.Decorate<ICommandHandler<ForgotPasswordCommand>>(
            (handler, serviceProvider) =>
                new RateLimitCommandDecorator<ForgotPasswordCommand>(
                    handler,
                    serviceProvider.GetRequiredService<IRateLimitService>(),
                    serviceProvider.GetRequiredService<IRateLimitStrategy<ForgotPasswordCommand>>(),
                    serviceProvider.GetRequiredService<
                        ILogger<RateLimitCommandDecorator<ForgotPasswordCommand>>
                    >()
                )
        );

        return services;
    }

    /// <summary>
    /// Configures the rate limiting middleware
    /// </summary>
    public static WebApplication UseRateLimiting(
        this WebApplication app,
        bool enableMiddleware = true
    )
    {
        if (enableMiddleware)
        {
            // Add rate limiting middleware before authentication
            app.UseMiddleware<RateLimitMiddleware>();
        }

        return app;
    }
}
