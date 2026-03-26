using LogiTransPro.API.Models.DTOs.Ruta;

namespace LogiTransPro.API.Services.Ruta
{
    public interface IRutaService
    {
        // ======================================================
        // LISTADOS
        // ======================================================
        Task<List<RutaDTO>> GetAllAsync();
        Task<List<RutaDTO>> GetActivasAsync();

        // ======================================================
        // BÚSQUEDAS POR CAMPOS ÚNICOS DE NEGOCIO
        // ======================================================
        Task<RutaDTO?> GetByCodigoAsync(string codigoRuta);
        Task<List<RutaDTO>> GetByOrigenDestinoAsync(string origen, string destino);

        // ======================================================
        // CRUD USANDO CÓDIGO DE RUTA COMO IDENTIFICADOR
        // ======================================================
        Task<RutaDTO> CreateAsync(RutaDTO createDto);
        Task<bool> UpdateByCodigoAsync(string codigoRuta, RutaDTO updateDto);
        Task<bool> DeleteByCodigoAsync(string codigoRuta);

        // ======================================================
        // OPERACIONES ESPECÍFICAS
        // ======================================================
        Task<RutaOptimizadaDTO> OptimizarRutaAsync(OptimizarRutaDTO optimizarDto);
    }
}