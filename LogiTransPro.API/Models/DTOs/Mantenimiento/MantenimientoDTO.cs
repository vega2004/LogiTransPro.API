namespace LogiTransPro.API.Models.DTOs.Mantenimiento
{
    public class MantenimientoDTO
    {
        public int MantenimientoId { get; set; }
        public int VehiculoId { get; set; }
        public string VehiculoPlaca { get; set; } = string.Empty;
        public string TipoServicio { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaProgramada { get; set; }
        public DateTime? FechaRealizada { get; set; }
        public int? KilometrajeAlerta { get; set; }
        public int? KilometrajeActual { get; set; }
        public decimal? Costo { get; set; }
        public string Prioridad { get; set; } = string.Empty;
        public string Estatus { get; set; } = string.Empty;
        public string? NotasMecanico { get; set; }
        public string? TecnicoAsignado { get; set; }
        public List<ParteMantenimientoDTO> Partes { get; set; } = new();
    }
}