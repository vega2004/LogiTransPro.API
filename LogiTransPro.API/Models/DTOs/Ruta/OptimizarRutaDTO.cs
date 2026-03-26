using System.ComponentModel.DataAnnotations;

namespace LogiTransPro.API.Models.DTOs.Ruta
{
    public class OptimizarRutaDTO
    {
        [Required(ErrorMessage = "El origen es requerido")]
        [MaxLength(150, ErrorMessage = "El origen no puede exceder 150 caracteres")]
        public string Origen { get; set; } = string.Empty;

        [Required(ErrorMessage = "El destino es requerido")]
        [MaxLength(150, ErrorMessage = "El destino no puede exceder 150 caracteres")]
        public string Destino { get; set; } = string.Empty;

        [RegularExpression("^(distancia|tiempo)$", ErrorMessage = "Tipo de optimización debe ser 'distancia' o 'tiempo'")]
        public string? TipoOptimizacion { get; set; } = "distancia";

        public List<string>? PuntosIntermedios { get; set; }
    }
}