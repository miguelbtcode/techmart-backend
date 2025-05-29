#!/bin/bash
# update-namespaces.sh - Actualiza namespaces después de la reestructuración

set -e

PROJECT_PATH="${1:-src/AuthMicroservice.Application}"
BASE_NAMESPACE="AuthMicroservice.Application"

echo "🔧 Actualizando namespaces..."

# Verificar que estamos en el directorio correcto
if [ ! -f "AuthMicroservice.sln" ]; then
    echo "❌ No se encontró AuthMicroservice.sln. Ejecuta desde la raíz del proyecto."
    exit 1
fi

# Función para actualizar namespace en un archivo
update_namespace() {
    local file="$1"
    local old_namespace="$2"
    local new_namespace="$3"
    
    if [ -f "$file" ]; then
        # Actualizar declaración de namespace
        sed -i.bak "s|namespace $old_namespace|namespace $new_namespace|g" "$file"
        
        # Actualizar using statements
        sed -i.bak "s|using $old_namespace|using $new_namespace|g" "$file"
        
        # Limpiar archivos backup
        rm -f "$file.bak"
        
        echo "📝 Updated: $(basename "$file")"
    fi
}

# Función para procesar directorio y actualizar namespaces basado en la estructura
process_directory() {
    local dir="$1"
    
    find "$PROJECT_PATH/$dir" -name "*.cs" -type f | while read -r file; do
        # Calcular el namespace basado en la ruta del archivo
        local relative_path="${file#$PROJECT_PATH/}"
        local dir_path=$(dirname "$relative_path")
        local new_namespace="$BASE_NAMESPACE.${dir_path//\//.}"
        
        # Actualizar el namespace en el archivo
        if [ -f "$file" ]; then
            # Buscar el namespace actual en el archivo
            current_namespace=$(grep -o "namespace [A-Za-z0-9._]*" "$file" | head -1 | sed 's/namespace //')
            
            if [ ! -z "$current_namespace" ] && [ "$current_namespace" != "$new_namespace" ]; then
                sed -i.bak "s|namespace $current_namespace|namespace $new_namespace|g" "$file"
                echo "📝 Updated namespace in $(basename "$file"): $current_namespace -> $new_namespace"
            fi
            
            # Limpiar archivo backup
            rm -f "$file.bak"
        fi
    done
}

echo "🔄 Procesando archivos en Features..."
process_directory "Features"

echo "🔄 Actualizando using statements..."

# Mapeo de namespaces antiguos a nuevos para using statements
declare -A namespace_mappings=(
    ["AuthMicroservice.Application.Authentication.Commands"]="AuthMicroservice.Application.Features.Authentication.Commands"
    ["AuthMicroservice.Application.Authentication.Handlers"]="AuthMicroservice.Application.Features.Authentication.Commands"
    ["AuthMicroservice.Application.Registration.Commands"]="AuthMicroservice.Application.Features.UserManagement.Commands.RegisterUser"
    ["AuthMicroservice.Application.Registration.Handlers"]="AuthMicroservice.Application.Features.UserManagement.Commands.RegisterUser"
    ["AuthMicroservice.Application.UserProfile.Commands"]="AuthMicroservice.Application.Features.UserManagement.Commands"
    ["AuthMicroservice.Application.UserProfile.Queries"]="AuthMicroservice.Application.Features.UserManagement.Queries"
    ["AuthMicroservice.Application.UserProfile.Handlers"]="AuthMicroservice.Application.Features.UserManagement"
    ["AuthMicroservice.Application.EmailVerification.Commands"]="AuthMicroservice.Application.Features.EmailVerification.Commands"
    ["AuthMicroservice.Application.EmailVerification.Handlers"]="AuthMicroservice.Application.Features.EmailVerification.Commands"
    ["AuthMicroservice.Application.PasswordReset.Commands"]="AuthMicroservice.Application.Features.PasswordReset.Commands"
    ["AuthMicroservice.Application.PasswordReset.Handlers"]="AuthMicroservice.Application.Features.PasswordReset.Commands"
    ["AuthMicroservice.Application.SocialAuth.Commands"]="AuthMicroservice.Application.Features.SocialAuthentication.Commands"
    ["AuthMicroservice.Application.SocialAuth.Queries"]="AuthMicroservice.Application.Features.SocialAuthentication.Queries"
    ["AuthMicroservice.Application.SocialAuth.Handlers"]="AuthMicroservice.Application.Features.SocialAuthentication.Commands"
)

# Actualizar using statements en todos los archivos
find "$PROJECT_PATH" -name "*.cs" -type f | while read -r file; do
    for old_ns in "${!namespace_mappings[@]}"; do
        new_ns="${namespace_mappings[$old_ns]}"
        sed -i.bak "s|using $old_ns;|using $new_ns;|g" "$file"
    done
    rm -f "$file.bak"
done

# También actualizar archivos en la API que referencien los comandos
API_PATH="src/AuthMicroservice.API"
if [ -d "$API_PATH" ]; then
    echo "🔄 Actualizando referencias en API..."
    find "$API_PATH" -name "*.cs" -type f | while read -r file; do
        for old_ns in "${!namespace_mappings[@]}"; do
            new_ns="${namespace_mappings[$old_ns]}"
            sed -i.bak "s|using $old_ns;|using $new_ns;|g" "$file"
        done
        rm -f "$file.bak"
    done
fi

echo ""
echo "✅ Namespaces actualizados exitosamente!"
echo ""
echo "📋 Próximos pasos:"
echo "  1. Compilar para verificar: dotnet build"
echo "  2. Generar event handlers: ./scripts/generate-event-handlers.sh"
echo "  3. Si hay errores, revisar using statements manualmente"