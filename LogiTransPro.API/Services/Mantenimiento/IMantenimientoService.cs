using LogiTransPro.API.Models.DTOs.Mantenimiento;

namespace LogiTransPro.API.Services.Mantenimiento
{
    public interface IMantenimientoService
    {
        Task<List<MantenimientoDTO>> GetAllAsync();
        Task<List<MantenimientoDTO>> GetPendientesAsync();
        Task<List<MantenimientoDTO>> GetProximosAsync(int dias = 30);
        Task<List<MantenimientoDTO>> GetByPlacaVehiculoAsync(string placa);
        Task<MantenimientoDTO> CreateAsync(CrearMantenimientoDTO createDto);
        Task<bool> CompletarByPlacaAsync(string placa, int kilometrajeActual, decimal? costoReal, string notasMecanico);
        Task<bool> CancelarByPlacaAsync(string placa, string motivo);
    }
}