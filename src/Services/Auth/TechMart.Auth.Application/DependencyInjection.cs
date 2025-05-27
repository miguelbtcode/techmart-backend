using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TechMart.Auth.Application.Abstractions.Messaging;
using TechMart.Auth.Application.Behaviors;

namespace TechMart.Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register Command and Query Handlers
        RegisterHandlers(services, assembly);

        // Add Decorators
        AddDecorators(services);

        // Add FluentValidation
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        // Add AutoMapper if needed (optional)
        // services.AddAutoMapper(assembly);

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        // Register Command Handlers with return value
        services.Scan(scan =>
            scan.FromAssemblies(assembly)
                .AddClasses(
                    classes => classes.AssignableTo(typeof(ICommandHandler<,>)),
                    publicOnly: false
                )
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        // Register Command Handlers without return value
        services.Scan(scan =>
            scan.FromAssemblies(assembly)
                .AddClasses(
                    classes => classes.AssignableTo(typeof(ICommandHandler<>)),
                    publicOnly: false
                )
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        // Register Query Handlers
        services.Scan(scan =>
            scan.FromAssemblies(assembly)
                .AddClasses(
                    classes => classes.AssignableTo(typeof(IQueryHandler<,>)),
                    publicOnly: false
                )
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );
    }

    private static void AddDecorators(IServiceCollection services)
    {
        // Validation Decorators (se ejecutan primero)
        services.Decorate(
            typeof(ICommandHandler<,>),
            typeof(ValidationDecorator.CommandHandler<,>)
        );
        services.Decorate(
            typeof(ICommandHandler<>),
            typeof(ValidationDecorator.CommandBaseHandler<>)
        );

        // Logging Decorators (se ejecutan después de la validación)
        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        // Aquí podrías agregar más decorators si los necesitas:
        // - Performance/Timing Decorator
        // - Caching Decorator
        // - Authorization Decorator
        // - Transaction Decorator
    }

    /// <summary>
    /// Extension method para registrar decorators adicionales si es necesario
    /// </summary>
    public static IServiceCollection AddApplicationDecorator<TDecorator>(
        this IServiceCollection services
    )
        where TDecorator : class
    {
        // Ejemplo de cómo agregar decorators adicionales
        services.Decorate(typeof(ICommandHandler<,>), typeof(TDecorator));
        services.Decorate(typeof(ICommandHandler<>), typeof(TDecorator));
        services.Decorate(typeof(IQueryHandler<,>), typeof(TDecorator));

        return services;
    }
}
