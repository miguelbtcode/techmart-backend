FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia los .csproj (rutas relativas a este Dockerfile)
COPY ["TechMart.Auth.API/TechMart.Auth.API.csproj", "TechMart.Auth.API/"]
COPY ["TechMart.Auth.Application/TechMart.Auth.Application.csproj", "TechMart.Auth.Application/"]
COPY ["TechMart.Auth.Domain/TechMart.Auth.Domain.csproj", "TechMart.Auth.Domain/"]
COPY ["TechMart.Auth.Infrastructure/TechMart.Auth.Infrastructure.csproj", "TechMart.Auth.Infrastructure/"]

# Restore
RUN dotnet restore "TechMart.Auth.API/TechMart.Auth.API.csproj"

# Copia todo el código fuente
COPY . .

# Build
WORKDIR "/src/TechMart.Auth.API"
RUN dotnet build "TechMart.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TechMart.Auth.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Crear usuario no-root
RUN addgroup --gid 2000 techmart && \
    adduser --uid 1000 --gid 2000 --shell /bin/bash --disabled-password techmart && \
    chown -R techmart:techmart /app

USER techmart

ENTRYPOINT ["dotnet", "TechMart.Auth.API.dll"]

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/health || exit 1
