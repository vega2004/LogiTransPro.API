namespace LogiTransPro.API.Models.DTOs.Viaje
{
    public class ViajeDTO
    {
        // Propiedades de la entidad Viaje
        public int ViajeId { get; set; }
        public string NumeroViaje { get; set; } = string.Empty;
        public int OrdenCargaId { get; set; }
        public int VehiculoId { get; set; }
        public int ChoferId { get; set; }
        public int RutaId { get; set; }
        public DateTime FechaSalidaProgramada { get; set; }
        public DateTime? FechaLlegadaProgramada { get; set; }
        public DateTime? FechaSalidaReal { get; set; }
        public DateTime? FechaLlegadaReal { get; set; }
        public int? KilometrajeInicial { get; set; }
        public int? KilometrajeFinal { get; set; }
        public decimal? ConsumoCombustible { get; set; }
        public string Estatus { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Propiedades de navegación (para mostrar información adicional)
        public string NumeroOrden { get; set; } = string.Empty;
        public string VehiculoPlaca { get; set; } = string.Empty;
        public string ChoferNombre { get; set; } = string.Empty;
        public string RutaNombre { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;

        // Propiedades calculadas
        public int? KilometrajeRecorrido => KilometrajeFinal.HasValue && KilometrajeInicial.HasValue
            ? KilometrajeFinal - KilometrajeInicial
            : null;

        public string TiempoTranscurrido
        {
            get
            {
                if (FechaSalidaReal.HasValue && FechaLlegadaReal.HasValue)
                {
                    var duracion = FechaLlegadaReal.Value - FechaSalidaReal.Value;
                    return $"{duracion.Hours}h {duracion.Minutes}m";
                }
                return "No iniciado";
            }
        }

        public bool EstaRetrasado
        {
            get
            {
                if (FechaLlegadaProgramada.HasValue && FechaLlegadaReal.HasValue)
                {
                    return FechaLlegadaReal > FechaLlegadaProgramada;
                }
                return false;
            }
        }
    }
}