using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using TechMart.SharedKernel.Behaviors;

namespace TechMart.SharedKernel.Extensions.ServiceCollection;

/// <summary>
/// Extension methods for configuring MediatR with TechMart behaviors.
/// </summary>
public static class MediatRExtensions
{
    /// <summary>
    /// Adds MediatR with TechMart behaviors to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for handlers.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartMediatR(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            
            // Register behaviors in order (outer to inner)
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionResultBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationResultBehavior<,>));
        });

        return services;
    }

    /// <summary>
    /// Adds MediatR with custom behavior configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureBehaviors">Action to configure behaviors.</param>
    /// <param name="assemblies">The assemblies to scan for handlers.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartMediatR(
        this IServiceCollection services, 
        Action<IServiceCollection> configureBehaviors,
        params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });

        configureBehaviors(services);
        return services;
    }

    /// <summary>
    /// Adds minimal MediatR configuration for high-performance scenarios.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for handlers.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartMediatRMinimal(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            
            // Only essential behaviors for production
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionResultBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationResultBehavior<,>));
        });

        return services;
    }

    /// <summary>
    /// Adds development MediatR configuration with detailed behaviors.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for handlers.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartMediatRDevelopment(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            
            // All behaviors for development
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DetailedPerformanceBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}