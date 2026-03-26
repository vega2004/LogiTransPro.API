using System.ComponentModel.DataAnnotations;

namespace LogiTransPro.API.Models.DTOs.Mantenimiento
{
    public class CrearMantenimientoDTO
    {
        [Required(ErrorMessage = "El vehículo es requerido")]
        public int VehiculoId { get; set; }

        [Required(ErrorMessage = "El tipo de servicio es requerido")]
        [RegularExpression("^[PC]$", ErrorMessage = "Tipo servicio debe ser P (Preventivo) o C (Correctivo)")]
        public string TipoServicio { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha programada es requerida")]
        [DataType(DataType.Date, ErrorMessage = "Formato de fecha inválido")]
        public DateTime FechaProgramada { get; set; }

        [Range(0, 999999, ErrorMessage = "El kilometraje de alerta debe ser mayor o igual a 0")]
        public int? KilometrajeAlerta { get; set; }

        [Range(0, 999999, ErrorMessage = "El costo debe ser mayor o igual a 0")]
        public decimal? Costo { get; set; }

        [Required(ErrorMessage = "La prioridad es requerida")]
        [RegularExpression("^(Baja|Media|Alta|Critica)$", ErrorMessage = "Prioridad debe ser Baja, Media, Alta o Critica")]
        public string Prioridad { get; set; } = "Media";

        [MaxLength(100, ErrorMessage = "El técnico asignado no puede exceder 100 caracteres")]
        public string? TecnicoAsignado { get; set; }

        public List<ParteMantenimientoDTO> Partes { get; set; } = new();
    }

}