using LogiTransPro.API.Models.DTOs.Auth;

namespace LogiTransPro.API.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDTO> LoginAsync(LoginDTO loginDto);
        Task<LoginResponseDTO> RegisterAsync(RegisterDTO registerDto);
        Task<LoginResponseDTO> RefreshTokenAsync(RefreshTokenDTO refreshTokenDto);
        Task<bool> LogoutAsync(int usuarioId);
        Task<bool> ChangePasswordAsync(int usuarioId, string contrasenaActual, string nuevaContrasena);
    }
}