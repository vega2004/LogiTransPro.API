namespace LogiTransPro.API.Models.DTOs.Ruta
{
    public class RutaOptimizadaDTO
    {
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public decimal DistanciaTotal { get; set; }
        public int TiempoEstimado { get; set; }
        public List<string> RutaCompleta { get; set; } = new();
        public string Recomendacion { get; set; } = string.Empty;
    }
}