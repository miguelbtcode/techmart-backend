# Script para crear el microservicio de autenticación
# Ejecutar en PowerShell o Terminal

# Crear la solución y proyecto
dotnet new sln -n AuthMicroservice
dotnet new webapi -n AuthMicroservice -o src/AuthMicroservice -f net9.0 --use-minimal-apis false
dotnet sln add src/AuthMicroservice/AuthMicroservice.csproj

# Crear estructura de carpetas
cd src/AuthMicroservice
mkdir Controllers Services Repositories Models Models/Entities Models/DTOs Data Infrastructure Validators

# Instalar paquetes NuGet necesarios
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.1.0
dotnet add package BCrypt.Net-Next --version 4.0.3
dotnet add package FastEndpoints --version 5.30.0
dotnet add package FastEndpoints.Swagger --version 5.30.0
dotnet add package Microsoft.AspNetCore.RateLimiting --version 9.0.0

# Crear archivos base (se crearán vacíos, los llenaremos después)
touch Models/Entities/User.cs
touch Models/Entities/RefreshToken.cs
touch Models/DTOs/LoginRequest.cs
touch Models/DTOs/LoginResponse.cs
touch Models/DTOs/RegisterRequest.cs
touch Models/DTOs/RefreshTokenRequest.cs
touch Data/AuthDbContext.cs
touch Repositories/IUserRepository.cs
touch Repositories/UserRepository.cs
touch Services/IAuthService.cs
touch Services/AuthService.cs
touch Infrastructure/JwtSettings.cs
touch Infrastructure/JwtTokenGenerator.cs
touch Controllers/AuthEndpoints.cs
touch Validators/LoginRequestValidator.cs
touch Validators/RegisterRequestValidator.cs

echo "✅ Proyecto creado exitosamente!"
echo "📁 Estructura de carpetas lista"
echo "📦 Paquetes NuGet instalados"
echo ""
echo "Siguiente paso: Implementar las clases y configuraciones"