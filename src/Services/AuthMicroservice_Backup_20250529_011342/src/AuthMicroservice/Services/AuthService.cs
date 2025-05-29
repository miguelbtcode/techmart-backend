using AuthMicroservice.Infrastructure;
using AuthMicroservice.Models.DTOs;
using AuthMicroservice.Models.Entities;
using AuthMicroservice.Repositories;

namespace AuthMicroservice.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        JwtSettings jwtSettings
    )
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _jwtSettings = jwtSettings;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Verificar si el email ya existe
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "El email ya está registrado",
                };
            }

            // Verificar si el username ya existe
            if (await _userRepository.UsernameExistsAsync(request.Username))
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "El nombre de usuario ya está en uso",
                };
            }

            // Crear el hash de la contraseña
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Crear el usuario
            var user = new User
            {
                Email = request.Email.ToLower().Trim(),
                Username = request.Username.ToLower().Trim(),
                PasswordHash = passwordHash,
                FirstName = request.FirstName?.Trim(),
                LastName = request.LastName?.Trim(),
                IsEmailVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            // Guardar en la base de datos
            var createdUser = await _userRepository.CreateAsync(user);

            // Mapear a DTO para respuesta
            var userDto = new UserDto
            {
                Id = createdUser.Id,
                Email = createdUser.Email,
                Username = createdUser.Username,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                IsEmailVerified = createdUser.IsEmailVerified,
                CreatedAt = createdUser.CreatedAt,
            };

            return new RegisterResponse
            {
                Success = true,
                Message = "Usuario registrado exitosamente",
                User = userDto,
            };
        }
        catch (Exception ex)
        {
            // Log del error (aquí podrías usar ILogger)
            return new RegisterResponse
            {
                Success = false,
                Message = "Error interno del servidor. Intente nuevamente.",
            };
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            // Buscar usuario por email o username
            User? user = null;

            if (request.EmailOrUsername.Contains("@"))
            {
                user = await _userRepository.GetByEmailAsync(request.EmailOrUsername);
            }
            else
            {
                user = await _userRepository.GetByUsernameAsync(request.EmailOrUsername);
            }

            // Verificar si el usuario existe
            if (user == null)
            {
                return new LoginResponse { Success = false, Message = "Credenciales inválidas" };
            }

            // Verificar si el usuario está activo
            if (!user.IsActive)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Cuenta desactivada. Contacte al administrador.",
                };
            }

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new LoginResponse { Success = false, Message = "Credenciales inválidas" };
            }

            // Generar tokens
            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();

            // Crear refresh token en BD
            var refreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(
                    request.RememberMe ? 30 : _jwtSettings.RefreshTokenExpirationDays
                ),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
            };

            await _refreshTokenRepository.CreateAsync(refreshToken);

            // Limpiar tokens expirados (tarea en background ideal)
            _ = Task.Run(async () => await _refreshTokenRepository.CleanupExpiredTokensAsync());

            // Mapear usuario para respuesta
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsEmailVerified = user.IsEmailVerified,
                CreatedAt = user.CreatedAt,
            };

            return new LoginResponse
            {
                Success = true,
                Message = "Login exitoso",
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                User = userDto,
            };
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Error interno del servidor. Intente nuevamente.",
            };
        }
    }

    public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            // Buscar refresh token
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (refreshToken == null)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Refresh token inválido o expirado",
                };
            }

            // Generar nuevo access token
            var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(refreshToken.User);
            var newRefreshTokenValue = _jwtTokenGenerator.GenerateRefreshToken();

            // Revocar el refresh token actual
            await _refreshTokenRepository.RevokeTokenAsync(request.RefreshToken);

            // Crear nuevo refresh token
            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenValue,
                UserId = refreshToken.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
            };

            await _refreshTokenRepository.CreateAsync(newRefreshToken);

            return new RefreshTokenResponse
            {
                Success = true,
                Message = "Token renovado exitosamente",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            };
        }
        catch (Exception ex)
        {
            return new RefreshTokenResponse
            {
                Success = false,
                Message = "Error interno del servidor. Intente nuevamente.",
            };
        }
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        try
        {
            return await _refreshTokenRepository.RevokeTokenAsync(refreshToken);
        }
        catch
        {
            return false;
        }
    }
}
