using LogiTransPro.API.Models.DTOs.Cliente;

namespace LogiTransPro.API.Services.Cliente
{
    public interface IClienteService
    {
        // ======================================================
        // LISTADOS
        // ======================================================
        Task<List<ClienteDTO>> GetAllAsync();
        Task<List<ClienteDTO>> GetActivosAsync();

        // ======================================================
        // BÚSQUEDAS POR CAMPOS ÚNICOS DE NEGOCIO
        // ======================================================
        Task<ClienteDTO?> GetByRfcAsync(string rfc);

        // ======================================================
        // CRUD USANDO RFC COMO IDENTIFICADOR
        // ======================================================
        Task<ClienteDTO> CreateAsync(CrearClienteDTO createDto);
        Task<bool> UpdateByRfcAsync(string rfc, ClienteDTO updateDto);
        Task<bool> DeleteByRfcAsync(string rfc);
    }
}