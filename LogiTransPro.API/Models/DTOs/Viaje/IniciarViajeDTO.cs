using System.ComponentModel.DataAnnotations;

namespace LogiTransPro.API.Models.DTOs.Viaje
{
    public class IniciarViajeDTO
    {
        [Required(ErrorMessage = "El kilometraje inicial es requerido")]
        [Range(0, 999999, ErrorMessage = "El kilometraje debe ser mayor o igual a 0")]
        public int KilometrajeInicial { get; set; }

        [Required(ErrorMessage = "El ID del usuario es requerido")]
        public int UsuarioId { get; set; }
    }
}