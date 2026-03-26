using System.ComponentModel.DataAnnotations;

namespace LogiTransPro.API.Models.DTOs.Viaje
{
    public class FinalizarViajeDTO
    {
        [Required(ErrorMessage = "El kilometraje final es requerido")]
        [Range(0, 999999, ErrorMessage = "El kilometraje debe ser mayor o igual a 0")]
        public int KilometrajeFinal { get; set; }

        [Required(ErrorMessage = "El consumo de combustible es requerido")]
        [Range(0, 9999, ErrorMessage = "El consumo debe ser mayor o igual a 0")]
        public decimal ConsumoCombustible { get; set; }
    }
}