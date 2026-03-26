namespace LogiTransPro.API.Models.DTOs.Dashboard
{
    public class KPIDTO
    {
        public int TotalViajesMes { get; set; }
        public decimal TasaCumplimiento { get; set; }
        public decimal ConsumoPromedioKM { get; set; }
        public decimal KilometrosRecorridosMes { get; set; }
        public decimal CostoPromedioPorViaje { get; set; }
        public int EntregasATiempo { get; set; }
        public int EntregasTarde { get; set; }
        public decimal PorcentajeCumplimiento => TotalViajesMes > 0
            ? (decimal)EntregasATiempo / TotalViajesMes * 100
            : 0;
    }
}