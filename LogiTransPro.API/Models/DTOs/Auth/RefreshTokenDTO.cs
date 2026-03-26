using System.ComponentModel.DataAnnotations;

namespace LogiTransPro.API.Models.DTOs.Auth
{
    public class RefreshTokenDTO
    {
        [Required(ErrorMessage = "El refresh token es requerido")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}