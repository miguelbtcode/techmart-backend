FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/AuthMicroservice.API/AuthMicroservice.API.csproj", "src/AuthMicroservice.API/"]
COPY ["src/AuthMicroservice.Application/AuthMicroservice.Application.csproj", "src/AuthMicroservice.Application/"]
COPY ["src/AuthMicroservice.Domain/AuthMicroservice.Domain.csproj", "src/AuthMicroservice.Domain/"]
COPY ["src/AuthMicroservice.Infrastructure/AuthMicroservice.Infrastructure.csproj", "src/AuthMicroservice.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/AuthMicroservice.API/AuthMicroservice.API.csproj"

# Copy all source code
COPY . .
WORKDIR "/src/src/AuthMicroservice.API"

# Build the application
RUN dotnet build "AuthMicroservice.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AuthMicroservice.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

ENTRYPOINT ["dotnet", "AuthMicroservice.API.dll"]
