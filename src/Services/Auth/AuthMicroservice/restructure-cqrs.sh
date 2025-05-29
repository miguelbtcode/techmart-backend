#!/bin/bash
# restructure-cqrs.sh - Reorganiza la estructura CQRS con bounded contexts

set -e

PROJECT_PATH="${1:-src/AuthMicroservice.Application}"
BASE_DIR="$(pwd)"

echo "🏗️ Reorganizando estructura CQRS..."
echo "📁 Directorio base: $BASE_DIR"
echo "📁 Proyecto: $PROJECT_PATH"

# Verificar que estamos en el directorio correcto
if [ ! -f "AuthMicroservice.sln" ]; then
    echo "❌ No se encontró AuthMicroservice.sln. Ejecuta desde la raíz del proyecto."
    exit 1
fi

# Función para mover archivos si existen
move_if_exists() {
    local source="$1"
    local dest="$2"
    local source_path="$PROJECT_PATH/$source"
    local dest_path="$PROJECT_PATH/$dest"
    
    echo "🔍 Buscando: $source_path"
    
    if [ -f "$source_path" ]; then
        dest_dir=$(dirname "$dest_path")
        mkdir -p "$dest_dir"
        mv "$source_path" "$dest_path"
        echo "✅ Moved: $source -> $dest"
    else
        echo "⚠️  Not found: $source"
    fi
}

echo ""
echo "📦 Moviendo archivos existentes..."

# Authentication files
move_if_exists "Authentication/Commands/LoginCommand.cs" "Features/Authentication/Commands/Login/LoginCommand.cs"
move_if_exists "Authentication/Commands/LoginCommandValidator.cs" "Features/Authentication/Commands/Login/LoginCommandValidator.cs"
move_if_exists "Authentication/Handlers/LoginHandler.cs" "Features/Authentication/Commands/Login/LoginCommandHandler.cs"
move_if_exists "Authentication/Commands/LogoutCommand.cs" "Features/Authentication/Commands/Logout/LogoutCommand.cs"
move_if_exists "Authentication/Handlers/LogoutHandler.cs" "Features/Authentication/Commands/Logout/LogoutCommandHandler.cs"
move_if_exists "Authentication/Commands/RefreshTokenCommand.cs" "Features/Authentication/Commands/RefreshToken/RefreshTokenCommand.cs"
move_if_exists "Authentication/Handlers/RefreshTokenHandler.cs" "Features/Authentication/Commands/RefreshToken/RefreshTokenCommandHandler.cs"

# Registration/UserManagement files
move_if_exists "Registration/Commands/RegisterCommand.cs" "Features/UserManagement/Commands/RegisterUser/RegisterUserCommand.cs"
move_if_exists "Registration/Handlers/RegisterHandler.cs" "Features/UserManagement/Commands/RegisterUser/RegisterUserCommandHandler.cs"

# UserProfile files
move_if_exists "UserProfile/Commands/UpdateProfileCommand.cs" "Features/UserManagement/Commands/UpdateProfile/UpdateProfileCommand.cs"
move_if_exists "UserProfile/Commands/ChangePasswordCommand.cs" "Features/UserManagement/Commands/ChangePassword/ChangePasswordCommand.cs"
move_if_exists "UserProfile/Queries/GetUserProfileQuery.cs" "Features/UserManagement/Queries/GetUserProfile/GetUserProfileQuery.cs"
move_if_exists "UserProfile/Handlers/GetUserProfileHandler.cs" "Features/UserManagement/Queries/GetUserProfile/GetUserProfileQueryHandler.cs"
move_if_exists "UserProfile/Handlers/UserProfileHandlers.cs" "Features/UserManagement/Commands/UpdateProfile/UpdateProfileCommandHandler.cs"
move_if_exists "UserProfile/Handlers/ChangePasswordHandler.cs" "Features/UserManagement/Commands/ChangePassword/ChangePasswordCommandHandler.cs"

# EmailVerification files
move_if_exists "EmailVerification/Commands/SendVerificationCommand.cs" "Features/EmailVerification/Commands/SendVerificationEmail/SendVerificationEmailCommand.cs"
move_if_exists "EmailVerification/Commands/VerifyEmailCommand.cs" "Features/EmailVerification/Commands/VerifyEmail/VerifyEmailCommand.cs"
move_if_exists "EmailVerification/Handlers/EmailVerificationHandlers.cs" "Features/EmailVerification/Commands/SendVerificationEmail/SendVerificationEmailCommandHandler.cs"

# PasswordReset files
move_if_exists "PasswordReset/Commands/ForgotPasswordCommand.cs" "Features/PasswordReset/Commands/ForgotPassword/ForgotPasswordCommand.cs"
move_if_exists "PasswordReset/Commands/ResetPasswordCommand.cs" "Features/PasswordReset/Commands/ResetPassword/ResetPasswordCommand.cs"
move_if_exists "PasswordReset/Handlers/PasswordResetHandlers.cs" "Features/PasswordReset/Commands/ForgotPassword/ForgotPasswordCommandHandler.cs"

# SocialAuth files
move_if_exists "SocialAuth/Commands/GoogleLoginCommand.cs" "Features/SocialAuthentication/Commands/GoogleLogin/GoogleLoginCommand.cs"
move_if_exists "SocialAuth/Commands/GitHubLoginCommand.cs" "Features/SocialAuthentication/Commands/GitHubLogin/GitHubLoginCommand.cs"
move_if_exists "SocialAuth/Queries/GetGoogleAuthUrlQuery.cs" "Features/SocialAuthentication/Queries/GetGoogleAuthUrlQuery.cs"
move_if_exists "SocialAuth/Queries/GetGitHubAuthUrlQuery.cs" "Features/SocialAuthentication/Queries/GetGitHubAuthUrlQuery.cs"
move_if_exists "SocialAuth/Handlers/GoogleLoginHandler.cs" "Features/SocialAuthentication/Commands/GoogleLogin/GoogleLoginCommandHandler.cs"
move_if_exists "SocialAuth/Handlers/AuthUrlHandlers.cs" "Features/SocialAuthentication/Queries/AuthUrlQueryHandlers.cs"

echo ""
echo "🧹 Limpiando carpetas vacías..."

# Función para limpiar carpetas vacías
cleanup_empty_dirs() {
    local dir="$1"
    if [ -d "$PROJECT_PATH/$dir" ]; then
        # Si la carpeta existe y está vacía, eliminarla
        if [ -z "$(find "$PROJECT_PATH/$dir" -mindepth 1 -type f)" ]; then
            rm -rf "$PROJECT_PATH/$dir"
            echo "🗑️  Removed empty folder: $dir"
        else
            echo "📁 Kept non-empty folder: $dir"
        fi
    fi
}

# Limpiar carpetas antiguas
cleanup_empty_dirs "Authentication/Commands"
cleanup_empty_dirs "Authentication/Handlers"
cleanup_empty_dirs "Authentication"
cleanup_empty_dirs "Registration/Commands"
cleanup_empty_dirs "Registration/Handlers"
cleanup_empty_dirs "Registration"
cleanup_empty_dirs "UserProfile/Commands"
cleanup_empty_dirs "UserProfile/Queries"
cleanup_empty_dirs "UserProfile/Handlers"
cleanup_empty_dirs "UserProfile"
cleanup_empty_dirs "EmailVerification/Commands"
cleanup_empty_dirs "EmailVerification/Handlers"
cleanup_empty_dirs "EmailVerification"
cleanup_empty_dirs "PasswordReset/Commands"
cleanup_empty_dirs "PasswordReset/Handlers"
cleanup_empty_dirs "PasswordReset"
cleanup_empty_dirs "SocialAuth/Commands"
cleanup_empty_dirs "SocialAuth/Queries"
cleanup_empty_dirs "SocialAuth/Handlers"
cleanup_empty_dirs "SocialAuth"

echo ""
echo "✅ Estructura CQRS reorganizada exitosamente!"
echo ""
echo "📋 Próximos pasos:"
echo "  1. Actualizar namespaces: ./scripts/update-namespaces.sh"
echo "  2. Generar event handlers: ./scripts/generate-event-handlers.sh" 
echo "  3. Compilar: dotnet build"