using System.ComponentModel.DataAnnotations;

namespace LogiTransPro.API.Models.DTOs.Auth
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        [MaxLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres")]
        public string Contrasena { get; set; } = string.Empty;
    }
}