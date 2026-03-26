using LogiTransPro.API.Models.DTOs.OrdenCarga;

namespace LogiTransPro.API.Services.OrdenCarga
{
    public interface IOrdenCargaService
    {
        // ======================================================
        // LISTADOS
        // ======================================================
        Task<List<OrdenCargaDTO>> GetAllAsync();
        Task<List<OrdenCargaDTO>> GetPendientesAsync();
        Task<List<OrdenCargaDTO>> GetUrgentesAsync();

        // ======================================================
        // BÚSQUEDAS POR CAMPOS ÚNICOS DE NEGOCIO
        // ======================================================
        Task<OrdenCargaDTO?> GetByNumeroOrdenAsync(string numeroOrden);
        Task<List<OrdenCargaDTO>> GetByClienteRfcAsync(string clienteRfc);

        // ======================================================
        // CRUD USANDO NÚMERO DE ORDEN COMO IDENTIFICADOR
        // ======================================================
        Task<OrdenCargaDTO> CreateAsync(CrearOrdenCargaDTO createDto);
        Task<bool> UpdateByNumeroOrdenAsync(string numeroOrden, OrdenCargaDTO updateDto);
        Task<bool> CancelarByNumeroOrdenAsync(string numeroOrden, string motivo);
        Task<bool> AsignarViajeByNumeroOrdenAsync(string numeroOrden, string numeroViaje);
    }
}