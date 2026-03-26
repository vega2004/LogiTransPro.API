namespace LogiTransPro.API.Models.DTOs.OrdenCarga
{
    public class OrdenCargaDTO
    {
        public int OrdenCargaId { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaRequerida { get; set; }
        public string DescripcionMercancia { get; set; } = string.Empty;
        public decimal PesoTotal { get; set; }
        public decimal? VolumenTotal { get; set; }
        public string? InstruccionesEspeciales { get; set; }
        public decimal? ValorDeclarado { get; set; }
        public string Estatus { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
    }
}