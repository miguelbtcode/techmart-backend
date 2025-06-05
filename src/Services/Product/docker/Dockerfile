FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy SharedKernel projects
COPY ["../src/Shared/TechMart.SharedKernel/src/TechMart.SharedKernel/TechMart.SharedKernel.csproj", "src/Shared/TechMart.SharedKernel/src/TechMart.SharedKernel/"]
COPY ["../src/Shared/TechMart.SharedKernel/src/TechMart.SharedKernel.Extensions/TechMart.SharedKernel.Extensions.csproj", "src/Shared/TechMart.SharedKernel/src/TechMart.SharedKernel.Extensions/"]

# Copy Product projects
COPY ["../src/TechMart.Product.Domain/TechMart.Product.Domain.csproj", "src/TechMart.Product.Domain/"]
COPY ["../src/TechMart.Product.Application/TechMart.Product.Application.csproj", "src/TechMart.Product.Application/"]
COPY ["../src/TechMart.Product.Infrastructure/TechMart.Product.Infrastructure.csproj", "src/TechMart.Product.Infrastructure/"]
COPY ["../src/TechMart.Product.Api/TechMart.Product.Api.csproj", "src/TechMart.Product.Api/"]

RUN dotnet restore "src/TechMart.Product.Api/TechMart.Product.Api.csproj"

COPY ../src/ ./src/
WORKDIR "/src/src/TechMart.Product.Api"
RUN dotnet build "TechMart.Product.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TechMart.Product.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechMart.Product.Api.dll"]