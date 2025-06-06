FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/Services/Auth/src/TechMart.Auth.Api/TechMart.Auth.Api.csproj", "src/Services/Auth/src/TechMart.Auth.Api/"]
COPY ["src/Services/Auth/src/TechMart.Auth.Application/TechMart.Auth.Application.csproj", "src/Services/Auth/src/TechMart.Auth.Application/"]
COPY ["src/Services/Auth/src/TechMart.Auth.Domain/TechMart.Auth.Domain.csproj", "src/Services/Auth/src/TechMart.Auth.Domain/"]
COPY ["src/Services/Auth/src/TechMart.Auth.Infrastructure/TechMart.Auth.Infrastructure.csproj", "src/Services/Auth/src/TechMart.Auth.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/Services/Auth/src/TechMart.Auth.Api/TechMart.Auth.Api.csproj"

# Copy all source code
COPY src/ ./src/
WORKDIR "/src/src/Services/Auth/src/TechMart.Auth.Api"

# Build the application
RUN dotnet build "TechMart.Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TechMart.Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

ENTRYPOINT ["dotnet", "TechMart.Auth.Api.dll"]
