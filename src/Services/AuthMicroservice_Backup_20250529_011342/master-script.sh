#!/bin/bash

# ================================
# SCRIPT MAESTRO DE MIGRACIÓN A CLEAN ARCHITECTURE (BASH)
# Ejecutar desde la raíz del proyecto AuthMicroservice
# ================================

PROJECT_PATH="."
SKIP_BACKUP=false

# Procesar argumentos
while [[ "$#" -gt 0 ]]; do
    case $1 in
        --project-path) PROJECT_PATH="$2"; shift ;;
        --skip-backup) SKIP_BACKUP=true ;;
        *) echo "❌ Opción desconocida: $1" ; exit 1 ;;
    esac
    shift
done

echo -e "\033[0;32m🚀 INICIANDO MIGRACIÓN A CLEAN ARCHITECTURE...\033[0m"
echo -e "\033[0;33m⏱️ Tiempo estimado: 15-20 minutos\033[0m"
echo ""

# Crear backup si no se especifica omitir
if [ "$SKIP_BACKUP" = false ]; then
    echo -e "\033[0;34m📦 Creando backup...\033[0m"
    TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
    BACKUP_PATH="../AuthMicroservice_Backup_$TIMESTAMP"
    cp -r "$PROJECT_PATH" "$BACKUP_PATH"
    echo -e "\033[0;32m✅ Backup creado en: $BACKUP_PATH\033[0m"
fi

# Navegar al directorio del proyecto
cd "$PROJECT_PATH" || exit

echo -e "\033[0;34m🏗️ Creando nueva estructura...\033[0m"

# Lista de directorios
directories=(
    "src/Domain/Entities"
    "src/Domain/ValueObjects"
    "src/Domain/Events"
    "src/Domain/Interfaces"
    "src/Application/Authentication/Commands"
    "src/Application/Authentication/Queries"
    "src/Application/Authentication/Handlers"
    "src/Application/Registration/Commands"
    "src/Application/Registration/Handlers"
    "src/Application/PasswordReset/Commands"
    "src/Application/PasswordReset/Handlers"
    "src/Application/SocialAuth/Commands"
    "src/Application/SocialAuth/Handlers"
    "src/Application/EmailVerification/Commands"
    "src/Application/EmailVerification/Handlers"
    "src/Application/Common/DTOs"
    "src/Application/Common/Results"
    "src/Application/Common/Behaviors"
    "src/Infrastructure/Data/Repositories"
    "src/Infrastructure/ExternalServices"
    "src/Infrastructure/Security"
    "src/API/Controllers"
    "src/API/Middlewares"
    "src/API/Extensions"
    "tests/Unit/Domain"
    "tests/Unit/Application"
    "tests/Unit/Infrastructure"
    "tests/Integration"
)

# Crear directorios
for dir in "${directories[@]}"; do
    mkdir -p "$dir"
done

echo -e "\033[0;32m✅ Estructura de directorios creada\033[0m"

# Instalar paquetes NuGet
echo -e "\033[0;34m📦 Instalando paquetes NuGet...\033[0m"

packages=(
    "MediatR@12.2.0"
    "MediatR.Extensions.Microsoft.DependencyInjection@11.1.0"
    "FluentValidation.AspNetCore@11.3.0"
    "Serilog.AspNetCore@8.0.0"
    "Serilog.Sinks.File@5.0.0"
    "Microsoft.AspNetCore.Authentication.Google@8.0.0"
    "Microsoft.AspNetCore.Authentication.OAuth@8.0.0"
    "SendGrid@9.29.3"
    "QRCoder@1.4.3"
    "Microsoft.Extensions.Caching.Memory@8.0.0"
)

for package in "${packages[@]}"; do
    name="${package%@*}"
    version="${package#*@}"
    echo -e "\033[0;33mInstalling $package...\033[0m"
    dotnet add package "$name" --version "$version" >/dev/null
done

echo -e "\033[0;32m✅ Paquetes instalados\033[0m"
echo ""

echo -e "\033[0;32m🎯 MIGRACIÓN COMPLETADA!\033[0m"
echo -e "\033[0;33m📁 Nueva estructura creada\033[0m"
echo -e "\033[0;33m📦 Paquetes instalados\033[0m"
echo ""
echo -e "\033[0;36m🚀 SIGUIENTE PASO: Ejecutar los scripts de generación de código\033[0m"
echo -e "\033[0;37m   ./generate-domain.sh"
echo -e "   ./generate-application.sh"
echo -e "   ./generate-infrastructure.sh"
echo -e "   ./generate-api.sh\033[0m"
