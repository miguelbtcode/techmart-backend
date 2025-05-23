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
COPY ["Services/Order/TechMart.Order.API/TechMart.Order.API.csproj", "Services/Order/TechMart.Order.API/"]
COPY ["Services/Order/TechMart.Order.Application/TechMart.Order.Application.csproj", "Services/Order/TechMart.Order.Application/"]
COPY ["Services/Order/TechMart.Order.Domain/TechMart.Order.Domain.csproj", "Services/Order/TechMart.Order.Domain/"]
COPY ["Services/Order/TechMart.Order.Infrastructure/TechMart.Order.Infrastructure.csproj", "Services/Order/TechMart.Order.Infrastructure/"]
COPY ["Shared/Common/TechMart.Shared.Common.csproj", "Shared/Common/"]
COPY ["Shared/EventBus/TechMart.Shared.EventBus.csproj", "Shared/EventBus/"]
COPY ["Shared/Logging/TechMart.Shared.Logging.csproj", "Shared/Logging/"]
COPY ["Shared/Security/TechMart.Shared.Security.csproj", "Shared/Security/"]
COPY ["Shared/Contracts/TechMart.Shared.Contracts.csproj", "Shared/Contracts/"]

# Restore dependencies
RUN dotnet restore "Services/Order/TechMart.Order.API/TechMart.Order.API.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/Services/Order/TechMart.Order.API"
RUN dotnet build "TechMart.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TechMart.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN addgroup --group techmart --gid 2000 \
    && adduser --uid 1000 --gid 2000 --shell /bin/bash --disabled-password techmart

# Change ownership and switch to non-root user
RUN chown -R techmart:techmart /app
USER techmart

ENTRYPOINT ["dotnet", "TechMart.Order.API.dll"]

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1