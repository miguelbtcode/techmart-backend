using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace TechMart.Product.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureProductApiPipeline(this WebApplication app)
    {
        app.UseGlobalExceptionHandler();
        
        // Development vs Production
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMart API v1");
                c.RoutePrefix = "swagger";
            });
        }

        // Security
        app.UseHttpsRedirection();
        
        // CORS
        app.UseCors("AllowDevOrigins");
        
        // Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Health checks
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    totalDuration = report.TotalDuration.ToString()
                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        });

        // Controllers
        app.MapControllers();

        return app;
    }
}