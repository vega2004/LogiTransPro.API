using System.ComponentModel.DataAnnotations;

namespace LogiTransPro.API.Models.DTOs.Mantenimiento
{
    public class ParteMantenimientoDTO
    {
        [Required(ErrorMessage = "El nombre de la parte es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre de la parte no puede exceder 100 caracteres")]
        public string NombreParte { get; set; } = string.Empty;

        [Range(1, 999, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; } = 1;

        [Range(0, 999999, ErrorMessage = "El costo unitario debe ser mayor o igual a 0")]
        public decimal? CostoUnitario { get; set; }
    }
}