using System.ComponentModel.DataAnnotations;

namespace LogiTransPro.API.Models.DTOs.OrdenCarga
{
    public class CrearOrdenCargaDTO
    {
        [Required(ErrorMessage = "El cliente es requerido")]
        public int ClienteId { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Formato de fecha inválido")]
        public DateTime? FechaRequerida { get; set; }

        [Required(ErrorMessage = "La descripción de la mercancía es requerida")]
        [MaxLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string DescripcionMercancia { get; set; } = string.Empty;

        [Required(ErrorMessage = "El peso total es requerido")]
        [Range(0.01, 100000, ErrorMessage = "El peso debe estar entre 0.01 y 100,000 kg")]
        public decimal PesoTotal { get; set; }

        [Range(0, 500, ErrorMessage = "El volumen debe estar entre 0 y 500 metros cúbicos")]
        public decimal? VolumenTotal { get; set; }

        [MaxLength(1000, ErrorMessage = "Las instrucciones especiales no pueden exceder 1000 caracteres")]
        public string? InstruccionesEspeciales { get; set; }

        [Range(0, 9999999, ErrorMessage = "El valor declarado debe ser mayor o igual a 0")]
        public decimal? ValorDeclarado { get; set; }

        [RegularExpression("^(Normal|Alta|Urgente)$", ErrorMessage = "Prioridad debe ser Normal, Alta o Urgente")]
        public string Prioridad { get; set; } = "Normal";
    }
}