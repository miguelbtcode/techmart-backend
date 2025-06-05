using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TechMart.SharedKernel.Extensions.ServiceCollection;

/// <summary>
/// Extension methods for configuring FluentValidation.
/// </summary>
public static class FluentValidationExtensions
{
    /// <summary>
    /// Adds FluentValidation with TechMart configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="assemblies">The assemblies to scan for validators.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartValidation(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddValidatorsFromAssemblies(assemblies);
        
        // Configure global validation settings
        ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
        {
            if (member != null)
            {
                return member.Name;
            }
            return null;
        };
        
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        return services;
    }

    /// <summary>
    /// Adds FluentValidation with custom configuration action.
    /// </summary>
    public static IServiceCollection AddTechMartValidation(
        this IServiceCollection services,
        Action configureGlobalOptions,
        params Assembly[] assemblies)
    {
        services.AddValidatorsFromAssemblies(assemblies);
        
        configureGlobalOptions();
        
        return services;
    }

    /// <summary>
    /// Adds automatic validator discovery and registration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="filter">Optional filter for validator types.</param>
    /// <param name="assemblies">The assemblies to scan for validators.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddTechMartValidationWithFilter(
        this IServiceCollection services,
        Func<Type, bool>? filter = null,
        params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var validatorTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)))
                .Where(filter ?? (_ => true));

            foreach (var validatorType in validatorTypes)
            {
                var interfaceType = validatorType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
                
                services.AddScoped(interfaceType, validatorType);
            }
        }
        
        ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
        {
            if (member != null)
            {
                return member.Name;
            }
            return null;
        };

        return services;
    }
}