using TechMart.Auth.API;
using TechMart.Auth.API.Extensions;
using TechMart.Auth.Application;
using TechMart.Auth.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configurar servicios de forma organizada
builder.Services.AddPresentation().AddApplication().AddInfrastructure(builder.Configuration);

// ✅ Configurar logging
builder.Host.ConfigureSerilog();

// ✅ Configurar autenticación y autorización
builder.Services.ConfigureAuthentication(builder.Configuration);

// ✅ Configurar CORS
builder.Services.ConfigureCors();

// ✅ Configurar Swagger
builder.Services.ConfigureSwaggerUI();

// ✅ Configurar health checks
builder.Services.ConfigureHealthChecks(builder.Configuration);

var app = builder.Build();

// ✅ Configurar pipeline de forma organizada
app.ConfigurePipeline();

// ✅ Mapear endpoints
app.MapEndpoints();

// ✅ Static files
app.UseStaticFiles();

app.UseSwaggerDocumentation(); // Swagger UI tradicional
app.UseScalarDocumentation(); // Scalar moderno

// ✅ Inicializar base de datos
await app.InitializeDatabaseAsync();

await app.RunAsync();
