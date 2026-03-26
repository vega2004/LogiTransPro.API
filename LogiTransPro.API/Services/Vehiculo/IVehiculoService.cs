using LogiTransPro.API.Models.DTOs.Vehiculo;

namespace LogiTransPro.API.Services.Vehiculo
{
    public interface IVehiculoService
    {
        // ======================================================
        // LISTADOS
        // ======================================================
        Task<List<VehiculoDTO>> GetAllAsync();
        Task<List<VehiculoDTO>> GetDisponiblesAsync();

        // ======================================================
        // BÚSQUEDAS POR CAMPOS ÚNICOS DE NEGOCIO
        // ======================================================
        Task<VehiculoDTO?> GetByPlacaAsync(string placa);
        Task<VehiculoDTO?> GetByVinAsync(string vin);

        // ======================================================
        // CRUD USANDO PLACA COMO IDENTIFICADOR
        // ======================================================
        Task<VehiculoDTO> CreateAsync(CrearVehiculoDTO createDto);
        Task<bool> UpdateByPlacaAsync(string placa, ActualizarVehiculoDTO updateDto);
        Task<bool> DeleteByPlacaAsync(string placa);

        // ======================================================
        // OPERACIONES ESPECÍFICAS USANDO PLACA
        // ======================================================
        Task<bool> ActualizarKilometrajeByPlacaAsync(string placa, int kilometraje);
        Task<bool> ActualizarCombustibleByPlacaAsync(string placa, decimal nivelCombustible);
    }
}