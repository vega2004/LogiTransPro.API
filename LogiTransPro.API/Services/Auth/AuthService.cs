using LogiTransPro.API.Data;
using LogiTransPro.API.Helpers;
using LogiTransPro.API.Models.DTOs.Auth;
using LogiTransPro.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LogiTransPro.API.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly LogiTransProDbContext _context;
        private readonly JWTService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            LogiTransProDbContext context,
            JWTService jwtService,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.CorreoElectronico == loginDto.CorreoElectronico);

            if (usuario == null)
            {
                await RegistrarIntentoLogin(loginDto.CorreoElectronico, false);
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            // Verificar si está bloqueado
            if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta > DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException($"Cuenta bloqueada hasta {usuario.BloqueadoHasta}");
            }

            // Verificar contraseña
            if (!PasswordHasher.VerifyPassword(loginDto.Contrasena, usuario.ContrasenaHash))
            {
                usuario.IntentosFallidos++;
                await _context.SaveChangesAsync();

                if (usuario.IntentosFallidos >= 5)
                {
                    usuario.BloqueadoHasta = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(30), DateTimeKind.Unspecified);
                    await _context.SaveChangesAsync();
                    throw new UnauthorizedAccessException("Demasiados intentos fallidos. Cuenta bloqueada por 30 minutos");
                }

                await RegistrarIntentoLogin(loginDto.CorreoElectronico, false);
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            // Resetear intentos fallidos
            usuario.IntentosFallidos = 0;
            usuario.UltimoAcceso = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            await _context.SaveChangesAsync();

            await RegistrarIntentoLogin(loginDto.CorreoElectronico, true);

            // Generar tokens
            var token = _jwtService.GenerateToken(usuario);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Guardar refresh token - CORREGIDO
            var refreshTokenEntity = new RefreshToken
            {
                UsuarioId = usuario.UsuarioId,
                Token = refreshToken,
                FechaExpiracion = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(7), DateTimeKind.Unspecified),
                Dispositivo = null,
                IpAddress = null
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new LoginResponseDTO
            {
                UsuarioId = usuario.UsuarioId,
                NombreCompleto = usuario.NombreCompleto,
                CorreoElectronico = usuario.CorreoElectronico,
                Rol = usuario.Rol?.NombreRol ?? "Operador",
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiracion = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60")), DateTimeKind.Unspecified)
            };
        }

        public async Task<LoginResponseDTO> RegisterAsync(RegisterDTO registerDto)
        {
            // Verificar si el correo ya existe
            var existe = await _context.Usuarios
                .AnyAsync(u => u.CorreoElectronico == registerDto.CorreoElectronico);

            if (existe)
            {
                throw new InvalidOperationException("El correo electrónico ya está registrado");
            }

            // Asignar rol por defecto si no se especifica
            int rolId = registerDto.RolId ?? await _context.Roles
                .Where(r => r.NombreRol == "Operador")
                .Select(r => r.RolId)
                .FirstOrDefaultAsync();

            var usuario = new Usuario
            {
                NombreCompleto = registerDto.NombreCompleto,
                CorreoElectronico = registerDto.CorreoElectronico,
                ContrasenaHash = PasswordHasher.HashPassword(registerDto.Contrasena),
                Telefono = registerDto.Telefono,
                RolId = rolId,
                Estado = "A",
                FechaRegistro = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                UltimoCambioPassword = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Nuevo usuario registrado: {Email}", usuario.CorreoElectronico);

            // Hacer login automático después del registro
            var loginDto = new LoginDTO
            {
                CorreoElectronico = registerDto.CorreoElectronico,
                Contrasena = registerDto.Contrasena
            };

            return await LoginAsync(loginDto);
        }

        public async Task<LoginResponseDTO> RefreshTokenAsync(RefreshTokenDTO refreshTokenDto)
        {
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.Usuario)
                .ThenInclude(u => u.Rol)
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken && !rt.Revocado);

            if (refreshToken == null)
            {
                throw new UnauthorizedAccessException("Refresh token inválido");
            }

            if (refreshToken.FechaExpiracion < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token expirado");
            }

            var usuario = refreshToken.Usuario;

            // Generar nuevo token
            var nuevoToken = _jwtService.GenerateToken(usuario);
            var nuevoRefreshToken = _jwtService.GenerateRefreshToken();

            // Revocar el refresh token actual
            refreshToken.Revocado = true;
            await _context.SaveChangesAsync();

            // Guardar nuevo refresh token - CORREGIDO
            var nuevoRefreshTokenEntity = new RefreshToken
            {
                UsuarioId = usuario.UsuarioId,
                Token = nuevoRefreshToken,
                FechaExpiracion = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(7), DateTimeKind.Unspecified),
                Dispositivo = null,
                IpAddress = null
            };

            _context.RefreshTokens.Add(nuevoRefreshTokenEntity);
            await _context.SaveChangesAsync();

            return new LoginResponseDTO
            {
                UsuarioId = usuario.UsuarioId,
                NombreCompleto = usuario.NombreCompleto,
                CorreoElectronico = usuario.CorreoElectronico,
                Rol = usuario.Rol?.NombreRol ?? "Operador",
                Token = nuevoToken,
                RefreshToken = nuevoRefreshToken,
                TokenExpiracion = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "60")), DateTimeKind.Unspecified)
            };
        }

        public async Task<bool> LogoutAsync(int usuarioId)
        {
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UsuarioId == usuarioId && !rt.Revocado)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.Revocado = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int usuarioId, string contrasenaActual, string nuevaContrasena)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            if (!PasswordHasher.VerifyPassword(contrasenaActual, usuario.ContrasenaHash))
            {
                throw new UnauthorizedAccessException("Contraseña actual incorrecta");
            }

            usuario.ContrasenaHash = PasswordHasher.HashPassword(nuevaContrasena);
            usuario.UltimoCambioPassword = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            usuario.RequiereCambioPassword = false;

            await _context.SaveChangesAsync();
            return true;
        }

        private async Task RegistrarIntentoLogin(string email, bool exitoso)
        {
            var intento = new LoginAttempt
            {
                CorreoElectronico = email,
                FechaHora = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                Exitoso = exitoso
            };

            _context.LoginAttempts.Add(intento);
            await _context.SaveChangesAsync();
        }
    }
}