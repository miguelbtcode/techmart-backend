using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TechMart.SharedKernel.Extensions.ServiceCollection;

namespace TechMart.Product.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR with TechMart behaviors
        services.AddTechMartMediatR(assembly);

        // FluentValidation
        services.AddTechMartValidation(assembly);

        // AutoMapper
        services.AddAutoMapper(assembly);

        return services;
    }
}