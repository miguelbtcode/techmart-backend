using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TechMart.Product.Application.Common.Behaviors;

namespace TechMart.Product.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR with TechMart behaviors
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        
        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);
        
        // Pipeline Behavior
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        // AutoMapper
        services.AddAutoMapper(assembly);

        return services;
    }
}