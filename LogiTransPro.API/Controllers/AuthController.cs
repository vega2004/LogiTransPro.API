using LogiTransPro.API.Attributes;
using LogiTransPro.API.Models.DTOs.Auth;
using LogiTransPro.API.Models.ViewModels;
using LogiTransPro.API.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace LogiTransPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Iniciar sesión
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDto);
                return Ok(ApiResponse<LoginResponseDTO>.Ok(response, "Login exitoso"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Registrar nuevo usuario
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                var response = await _authService.RegisterAsync(registerDto);
                return Ok(ApiResponse<LoginResponseDTO>.Ok(response, "Usuario registrado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Refrescar token JWT
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO refreshTokenDto)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(refreshTokenDto);
                return Ok(ApiResponse<LoginResponseDTO>.Ok(response, "Token refrescado exitosamente"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// Cerrar sesión
        /// </summary>
        [HttpPost("logout")]
        [AuthorizeRole]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var usuarioId = int.Parse(User.FindFirst("nameid")?.Value ?? "0");
            await _authService.LogoutAsync(usuarioId);
            return Ok(ApiResponse<bool>.Ok(true, "Sesión cerrada exitosamente"));
        }

        /// <summary>
        /// Cambiar contraseña
        /// </summary>
        [HttpPost("change-password")]
        [AuthorizeRole]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst("nameid")?.Value ?? "0");
                var result = await _authService.ChangePasswordAsync(usuarioId, changePasswordDto.ContrasenaActual, changePasswordDto.NuevaContrasena);
                return Ok(ApiResponse<bool>.Ok(result, "Contraseña cambiada exitosamente"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Error(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<object>.Error(ex.Message));
            }
        }
    }

    public class ChangePasswordDTO
    {
        public string ContrasenaActual { get; set; } = string.Empty;
        public string NuevaContrasena { get; set; } = string.Empty;
    }
}