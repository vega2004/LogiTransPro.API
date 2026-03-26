namespace LogiTransPro.API.Models.DTOs.Dashboard
{
    public class ProximoMantenimientoDTO
    {
        public string Placa { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public DateTime FechaProgramada { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public int DiasRestantes { get; set; }
    }
}
