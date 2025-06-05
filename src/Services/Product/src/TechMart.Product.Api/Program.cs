using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TechMart.Product.Application;
using TechMart.Product.Infrastructure;
using TechMart.Product.Infrastructure.Data.EntityFramework;
using TechMart.Product.Infrastructure.Data.EntityFramework.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add API Explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TechMart Product API",
        Version = "v1",
        Description = "Product management API for TechMart e-commerce platform",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "TechMart Team",
            Email = "support@techmart.com",
            Url = new Uri("https://techmart.com")
        }
    });

    // Si usas anotaciones [Produces], [Consumes], etc.
    options.EnableAnnotations();

    // (Opcional) Incluir comentarios XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDevOrigins", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add authentication and authorization
var jwtSection = builder.Configuration.GetSection("Jwt");

if (jwtSection.Exists())
{
    var secretKey = jwtSection.GetValue<string>("SecretKey");
    var issuer = jwtSection.GetValue<string>("Issuer");
    var audience = jwtSection.GetValue<string>("Audience");

    var key = Encoding.ASCII.GetBytes(secretKey);

    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = !string.IsNullOrEmpty(issuer),
                ValidIssuer = issuer,

                ValidateAudience = !string.IsNullOrEmpty(audience),
                ValidAudience = audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        // Puedes agregar políticas personalizadas aquí si quieres
        options.AddPolicy("DefaultPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
        });
    });
}

// Add Application layer
builder.Services.AddApplication();

// Add Infrastructure layer
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TechMart API v1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
    
    app.Use(async (context, next) =>
    {
        var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("TechMart.DetailedRequestLogging");
        var stopwatch = Stopwatch.StartNew();
        var traceId = context.TraceIdentifier;

        // Log request details
        logger.LogInformation(
            "Request started: {Method} {Path} - Headers: {@Headers} - TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            traceId);

        try
        {
            await next();
        }
        finally
        {
            stopwatch.Stop();
                
            logger.LogInformation(
                "Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {ElapsedMs}ms - TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                traceId);
        }
    });
}
else
{
    app.Use(async (context, next) =>
    {
        var loggerFactory = context.RequestServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("TechMart.RequestLogging");
        var stopwatch = Stopwatch.StartNew();
        var traceId = context.TraceIdentifier;

        logger.LogInformation(
            "Request started: {Method} {Path} - TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            traceId);

        try
        {
            await next();
        }
        finally
        {
            stopwatch.Stop();
                
            logger.LogInformation(
                "Request completed: {Method} {Path} - Status: {StatusCode} - Duration: {ElapsedMs}ms - TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                traceId);
        }
    });
}

// Global exception handling
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Add correlation ID
app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            
    context.Items["CorrelationId"] = correlationId;
    context.Response.Headers["X-Correlation-ID"] = correlationId;

    await next();
});

// Security headers and HTTPS
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowDevOrigins");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Seed database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        await DatabaseSeeder.SeedAllAsync(context);
        app.Logger.LogInformation("Database seeded successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while seeding the database");
    }
}

app.Run();