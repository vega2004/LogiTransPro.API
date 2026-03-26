using LogiTransPro.API.Models.DTOs.Viaje;

namespace LogiTransPro.API.Services.Viaje
{
    public interface IViajeService
    {
        // ======================================================
        // LISTADOS
        // ======================================================
        Task<List<ViajeDTO>> GetAllAsync();
        Task<List<ViajeDTO>> GetActivosAsync();
        Task<List<ViajeDTO>> GetProximosAsync(int dias = 7);

        // ======================================================
        // BÚSQUEDAS POR CAMPOS ÚNICOS DE NEGOCIO
        // ======================================================
        Task<ViajeDTO?> GetByNumeroViajeAsync(string numeroViaje);
        Task<List<ViajeDTO>> GetByPlacaVehiculoAsync(string placa);
        Task<List<ViajeDTO>> GetByEmailChoferAsync(string emailChofer);

        // ======================================================
        // CRUD USANDO NÚMERO DE VIAJE COMO IDENTIFICADOR
        // ======================================================
        Task<ViajeDTO> CreateAsync(CrearViajeDTO createDto);
        Task<bool> IniciarViajeByNumeroAsync(string numeroViaje, IniciarViajeDTO iniciarDto);
        Task<bool> FinalizarViajeByNumeroAsync(string numeroViaje, FinalizarViajeDTO finalizarDto);
        Task<bool> CancelarViajeByNumeroAsync(string numeroViaje, string motivo);
    }
}