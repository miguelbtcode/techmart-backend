using System.Text;
using AuthMicroservice.Application.Contracts.Jwt;
using AuthMicroservice.Domain.Interfaces;
using AuthMicroservice.Infrastructure.BackgroundJob;
using AuthMicroservice.Infrastructure.Data;
using AuthMicroservice.Infrastructure.Data.Repositories;
using AuthMicroservice.Infrastructure.ExternalServices;
using AuthMicroservice.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Retry;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AuthMicroservice.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Database Configuration
        services.AddDatabase(configuration);

        // Authentication & Security
        services.AddAuthentication(configuration);

        // External Services
        services.AddExternalServices(configuration);

        // Repository Pattern
        services.AddRepositories();

        // Infrastructure Services
        services.AddInfrastructureServices();

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found."
            );

        services.AddDbContext<AuthDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    );

                    sqlOptions.CommandTimeout(30);
                }
            );

            // Enable sensitive data logging only in development
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Health checks for database
        services
            .AddHealthChecks()
            .AddDbContextCheck<AuthDbContext>("database", tags: new[] { "db", "ready" });

        return services;
    }

    private static IServiceCollection AddAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // JWT Settings
        var jwtSettings = new JwtSettings();
        configuration.GetSection("JwtSettings").Bind(jwtSettings);

        // Validate JWT Settings
        ValidateJwtSettings(jwtSettings);
        services.AddSingleton(jwtSettings);

        // JWT Authentication
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
                    ),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                };

                // JWT Events for logging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Authentication failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine(
                            "Token validated for user: " + context.Principal?.Identity?.Name
                        );
                        return Task.CompletedTask;
                    },
                };
            });

        return services;
    }

    private static IServiceCollection AddExternalServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Email Service (SMTP Gmail)
        var emailSettings = new EmailSettings();
        configuration.GetSection("EmailSettings").Bind(emailSettings);

        ValidateEmailSettings(emailSettings);
        services.AddSingleton(emailSettings);
        services.AddScoped<IEmailService, EmailService>();

        // Social Authentication Services
        var socialAuthSettings = new SocialAuthSettings();
        configuration.GetSection("SocialAuth").Bind(socialAuthSettings);
        services.AddSingleton(socialAuthSettings);

        // HTTP Client for Social Auth
        services
            .AddHttpClient<SocialAuthService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("User-Agent", "$PROJECT_NAME/1.0");
            })
            .AddResilienceHandler("retry-policy", ConfigureRetryPolicy);

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Repository registrations with scoped lifetime
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Infrastructure services
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISocialAuthService, SocialAuthService>();

        // Background services for cleanup
        services.AddHostedService<TokenCleanupService>();

        return services;
    }

    // Helper method for HTTP retry policy
    private static void ConfigureRetryPolicy(ResiliencePipelineBuilder<HttpResponseMessage> builder)
    {
        builder.AddRetry(
            new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TaskCanceledException>()
                    .HandleResult(response => !response.IsSuccessStatusCode),
                Delay = TimeSpan.FromSeconds(1),
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    Console.WriteLine(
                        "Retry {RetryCount} after {Delay}ms",
                        args.AttemptNumber,
                        args.RetryDelay.TotalMilliseconds
                    );
                    return ValueTask.CompletedTask;
                },
            }
        );
    }

    // Validation methods
    private static void ValidateJwtSettings(JwtSettings jwtSettings)
    {
        if (string.IsNullOrEmpty(jwtSettings.SecretKey))
            throw new InvalidOperationException("JWT SecretKey is required.");

        if (jwtSettings.SecretKey.Length < 32)
            throw new InvalidOperationException(
                "JWT SecretKey must be at least 32 characters long."
            );

        if (string.IsNullOrEmpty(jwtSettings.Issuer))
            throw new InvalidOperationException("JWT Issuer is required.");

        if (string.IsNullOrEmpty(jwtSettings.Audience))
            throw new InvalidOperationException("JWT Audience is required.");

        if (jwtSettings.AccessTokenExpirationMinutes <= 0)
            throw new InvalidOperationException(
                "JWT AccessTokenExpirationMinutes must be greater than 0."
            );

        if (jwtSettings.RefreshTokenExpirationDays <= 0)
            throw new InvalidOperationException(
                "JWT RefreshTokenExpirationDays must be greater than 0."
            );
    }

    private static void ValidateEmailSettings(EmailSettings emailSettings)
    {
        if (string.IsNullOrEmpty(emailSettings.SmtpServer))
            throw new InvalidOperationException("Email SmtpServer is required.");

        if (string.IsNullOrEmpty(emailSettings.Username))
            throw new InvalidOperationException("Email Username is required.");

        if (string.IsNullOrEmpty(emailSettings.Password))
            throw new InvalidOperationException("Email Password is required.");

        if (string.IsNullOrEmpty(emailSettings.SenderEmail))
            throw new InvalidOperationException("Email SenderEmail is required.");

        if (string.IsNullOrEmpty(emailSettings.SenderName))
            throw new InvalidOperationException("Email SenderName is required.");

        if (emailSettings.SmtpPort <= 0)
            throw new InvalidOperationException("Email SmtpPort must be greater than 0.");
    }
}
