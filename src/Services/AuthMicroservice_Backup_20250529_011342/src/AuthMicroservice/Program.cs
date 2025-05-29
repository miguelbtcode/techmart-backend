using System.Text;
using AuthMicroservice.Data;
using AuthMicroservice.Infrastructure;
using AuthMicroservice.Repositories;
using AuthMicroservice.Services;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configurar JWT Settings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

// Configurar Entity Framework
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Registro de dependencias
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// Registro del servicio de limpieza de tokens expirados
builder.Services.AddHostedService<TokenCleanupService>();

// FastEndpoints
builder.Services.AddFastEndpoints();

// JWT Authentication
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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
        };
    });

builder.Services.AddAuthorization();

// Swagger para FastEndpoints
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Auth Microservice API";
        s.Version = "v1";
        s.Description = "Microservicio de autenticación simple y eficiente";
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

// Health Checks
builder
    .Services.AddHealthChecks()
    .AddCheck(
        "database",
        () =>
        {
            try
            {
                using var scope = builder.Services.BuildServiceProvider().CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                return context.Database.CanConnect()
                    ? HealthCheckResult.Healthy()
                    : HealthCheckResult.Unhealthy();
            }
            catch
            {
                return HealthCheckResult.Unhealthy();
            }
        }
    );

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(
        "AuthPolicy",
        opt =>
        {
            opt.PermitLimit = 10;
            opt.Window = TimeSpan.FromMinutes(1);
            opt.QueueProcessingOrder = System
                .Threading
                .RateLimiting
                .QueueProcessingOrder
                .OldestFirst;
            opt.QueueLimit = 5;
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRateLimiter();

// FastEndpoints middleware
app.UseFastEndpoints();

// Swagger

app.UseSwaggerGen();

// Health checks
app.MapHealthChecks("/health");

// Crear base de datos automáticamente en desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await context.Database.EnsureCreatedAsync();
}

app.Run();
