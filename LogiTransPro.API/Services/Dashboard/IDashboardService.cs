using LogiTransPro.API.Models.DTOs.Dashboard;

namespace LogiTransPro.API.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardDTO> GetDashboardDataAsync();
        Task<KPIDTO> GetKPIsAsync();
        Task<List<ProximoMantenimientoDTO>> GetProximosMantenimientosAsync(int top = 10);
        Task<int> GetOrdenesPendientesCountAsync();
        Task<int> GetViajesActivosCountAsync();
    }
}