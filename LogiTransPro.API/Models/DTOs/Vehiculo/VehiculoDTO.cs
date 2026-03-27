namespace LogiTransPro.API.Models.DTOs.Vehiculo
{
    public class VehiculoDTO
    {
        public int VehiculoId { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Vin { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Anio { get; set; }
        public decimal CapacidadCarga { get; set; }
        public decimal? CapacidadVolumen { get; set; }
        public int KilometrajeActual { get; set; }
        public string KilometrajeTexto { get; set; } = string.Empty;  // ← AGREGAR ESTA LÍNEA
        public decimal NivelCombustible { get; set; }
        public string EstadoMotor { get; set; } = string.Empty;
        public string EstadoGeneral { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
    }
}
