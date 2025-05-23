# Dockerfile Template para Microservicios .NET 8
# Este archivo debe ser copiado para cada servicio (Auth, Product, Order, etc.)

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["Services/Notification/TechMart.Notification.API/TechMart.Notification.API.csproj", "Services/Notification/TechMart.Notification.API/"]
COPY ["Services/Notification/TechMart.Notification.Application/TechMart.Notification.Application.csproj", "Services/Notification/TechMart.Notification.Application/"]
COPY ["Services/Notification/TechMart.Notification.Domain/TechMart.Notification.Domain.csproj", "Services/Notification/TechMart.Notification.Domain/"]
COPY ["Services/Notification/TechMart.Notification.Infrastructure/TechMart.Notification.Infrastructure.csproj", "Services/Notification/TechMart.Notification.Infrastructure/"]
COPY ["Shared/Common/TechMart.Shared.Common.csproj", "Shared/Common/"]
COPY ["Shared/EventBus/TechMart.Shared.EventBus.csproj", "Shared/EventBus/"]
COPY ["Shared/Logging/TechMart.Shared.Logging.csproj", "Shared/Logging/"]
COPY ["Shared/Security/TechMart.Shared.Security.csproj", "Shared/Security/"]
COPY ["Shared/Contracts/TechMart.Shared.Contracts.csproj", "Shared/Contracts/"]

# Restore dependencies
RUN dotnet restore "Services/Notification/TechMart.Notification.API/TechMart.Notification.API.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/Services/Notification/TechMart.Notification.API"
RUN dotnet build "TechMart.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TechMart.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN addgroup --group techmart --gid 2000 \
    && adduser --uid 1000 --gid 2000 --shell /bin/bash --disabled-password techmart

# Change ownership and switch to non-root user
RUN chown -R techmart:techmart /app
USER techmart

ENTRYPOINT ["dotnet", "TechMart.Notification.API.dll"]

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1