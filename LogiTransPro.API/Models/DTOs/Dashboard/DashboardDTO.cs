namespace LogiTransPro.API.Models.DTOs.Dashboard
{
    public class DashboardDTO
    {
        public int TotalVehiculos { get; set; }
        public int VehiculosEnRuta { get; set; }
        public int VehiculosDisponibles { get; set; }
        public int VehiculosEnMantenimiento { get; set; }
        public List<ProximoMantenimientoDTO> ProximosMantenimientos { get; set; } = new();
        public KPIDTO KPIs { get; set; } = new();
        public int OrdenesPendientes { get; set; }
        public int ViajesActivos { get; set; }
        public int AlertasCriticas { get; set; }
        public int AlertasMedia { get; set; }
        public int AlertasBaja { get; set; }
    }

}