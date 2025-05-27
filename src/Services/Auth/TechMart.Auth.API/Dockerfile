FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/Services/Auth/TechMart.Auth.API/TechMart.Auth.API.csproj", "src/Services/Auth/TechMart.Auth.API/"]
COPY ["src/Services/Auth/TechMart.Auth.Application/TechMart.Auth.Application.csproj", "src/Services/Auth/TechMart.Auth.Application/"]
COPY ["src/Services/Auth/TechMart.Auth.Domain/TechMart.Auth.Domain.csproj", "src/Services/Auth/TechMart.Auth.Domain/"]
COPY ["src/Services/Auth/TechMart.Auth.Infrastructure/TechMart.Auth.Infrastructure.csproj", "src/Services/Auth/TechMart.Auth.Infrastructure/"]

# Restore packages
RUN dotnet restore "src/Services/Auth/TechMart.Auth.API/TechMart.Auth.API.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/src/Services/Auth/TechMart.Auth.API"
RUN dotnet build "TechMart.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TechMart.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Copy published application
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "TechMart.Auth.API.dll"]