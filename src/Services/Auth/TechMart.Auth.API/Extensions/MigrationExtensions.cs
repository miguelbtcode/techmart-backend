using Microsoft.EntityFrameworkCore;
using TechMart.Auth.Infrastructure.Persistence;

namespace TechMart.Auth.API.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using AuthDbContext dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

        dbContext.Database.Migrate();
    }
}
