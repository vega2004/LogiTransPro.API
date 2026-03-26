using System.ComponentModel.DataAnnotations;

namespace LogiTransPro.API.Models.DTOs.Cliente
{
    public class CrearClienteDTO
    {
        [Required(ErrorMessage = "El RFC es requerido")]
        [StringLength(13, MinimumLength = 12, ErrorMessage = "El RFC debe tener 12 o 13 caracteres")]
        [RegularExpression(@"^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$",
            ErrorMessage = "Formato de RFC inválido")]
        public string Rfc { get; set; } = string.Empty;

        [Required(ErrorMessage = "La razón social es requerida")]
        [MaxLength(150, ErrorMessage = "La razón social no puede exceder 150 caracteres")]
        public string NombreRazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [MaxLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido")]
        [MaxLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        public string? CorreoElectronico { get; set; }

        [MaxLength(100, ErrorMessage = "La persona de contacto no puede exceder 100 caracteres")]
        public string? PersonaContacto { get; set; }

        [MaxLength(255, ErrorMessage = "La dirección no puede exceder 255 caracteres")]
        public string? Direccion { get; set; }
    }
}