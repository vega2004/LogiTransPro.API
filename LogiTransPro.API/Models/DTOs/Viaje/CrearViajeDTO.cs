using System.ComponentModel.DataAnnotations;

namespace LogiTransPro.API.Models.DTOs.Viaje
{
    public class CrearViajeDTO
    {
        [Required(ErrorMessage = "La orden de carga es requerida")]
        public int OrdenCargaId { get; set; }

        [Required(ErrorMessage = "El vehículo es requerido")]
        public int VehiculoId { get; set; }

        [Required(ErrorMessage = "El chofer es requerido")]
        public int ChoferId { get; set; }

        [Required(ErrorMessage = "La ruta es requerida")]
        public int RutaId { get; set; }

        [Required(ErrorMessage = "La fecha de salida programada es requerida")]
        [DataType(DataType.DateTime, ErrorMessage = "Formato de fecha inválido")]
        public DateTime FechaSalidaProgramada { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "Formato de fecha inválido")]
        public DateTime? FechaLlegadaProgramada { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string? Observaciones { get; set; }
    }
}