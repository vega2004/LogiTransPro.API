using System.ComponentModel.DataAnnotations;

namespace LogiTransPro.API.Models.DTOs.Vehiculo
{
    public class ActualizarVehiculoDTO
    {
        [StringLength(10, MinimumLength = 5, ErrorMessage = "La placa debe tener entre 5 y 10 caracteres")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "La placa solo puede contener letras mayúsculas y números")]
        public string? Placa { get; set; }

        [StringLength(17, MinimumLength = 17, ErrorMessage = "El VIN debe tener exactamente 17 caracteres")]
        [RegularExpression(@"^[A-HJ-NPR-Z0-9]{17}$", ErrorMessage = "El VIN tiene formato inválido")]
        public string? Vin { get; set; }

        [MaxLength(50, ErrorMessage = "La marca no puede exceder 50 caracteres")]
        public string? Marca { get; set; }

        [MaxLength(50, ErrorMessage = "El modelo no puede exceder 50 caracteres")]
        public string? Modelo { get; set; }

        [Range(1980, 2026, ErrorMessage = "El año debe estar entre 1980 y 2026")]
        public int? Anio { get; set; }

        [Range(0.01, 100, ErrorMessage = "La capacidad de carga debe estar entre 0.01 y 100 toneladas")]
        public decimal? CapacidadCarga { get; set; }

        [Range(0, 500, ErrorMessage = "El volumen debe estar entre 0 y 500 metros cúbicos")]
        public decimal? CapacidadVolumen { get; set; }

        [Range(0, 999999, ErrorMessage = "El kilometraje debe ser mayor o igual a 0")]
        public int? KilometrajeActual { get; set; }

        [Range(0, 100, ErrorMessage = "El nivel de combustible debe estar entre 0 y 100%")]
        public decimal? NivelCombustible { get; set; }

        [MaxLength(50, ErrorMessage = "El estado del motor no puede exceder 50 caracteres")]
        public string? EstadoMotor { get; set; }

        [RegularExpression("^[DRM]$", ErrorMessage = "Estado general debe ser D (Disponible), R (En Ruta) o M (Mantenimiento)")]
        public string? EstadoGeneral { get; set; }

        public bool? Activo { get; set; }
    }
}