namespace LogiTransPro.API.Models.DTOs.Ruta
{
    public class RutaDTO
    {
        public int RutaId { get; set; }
        public string CodigoRuta { get; set; } = string.Empty;
        public string? NombreRuta { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public decimal DistanciaEstimada { get; set; }
        public int? TiempoEstimado { get; set; }
        public bool Activo { get; set; }
    }
}