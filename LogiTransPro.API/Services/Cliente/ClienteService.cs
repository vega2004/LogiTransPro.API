using AutoMapper;
using LogiTransPro.API.Data;
using LogiTransPro.API.Models.DTOs.Cliente;
using LogiTransPro.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiTransPro.API.Services.Cliente
{
    public class ClienteService : IClienteService
    {
        private readonly LogiTransProDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ClienteService> _logger;

        public ClienteService(LogiTransProDbContext context, IMapper mapper, ILogger<ClienteService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        public async Task<List<ClienteDTO>> GetAllAsync()
        {
            var clientes = await _context.Clientes
                .Where(c => c.Activo)
                .OrderBy(c => c.NombreRazonSocial)
                .ToListAsync();

            return _mapper.Map<List<ClienteDTO>>(clientes);
        }

        public async Task<List<ClienteDTO>> GetActivosAsync()
        {
            var clientes = await _context.Clientes
                .Where(c => c.Activo)
                .OrderBy(c => c.NombreRazonSocial)
                .ToListAsync();

            return _mapper.Map<List<ClienteDTO>>(clientes);
        }

        // ======================================================
        // BÚSQUEDA POR RFC
        // ======================================================

        public async Task<ClienteDTO?> GetByRfcAsync(string rfc)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Rfc == rfc && c.Activo);

            return cliente != null ? _mapper.Map<ClienteDTO>(cliente) : null;
        }

        // ======================================================
        // CRUD USANDO RFC COMO IDENTIFICADOR
        // ======================================================

        public async Task<ClienteDTO> CreateAsync(CrearClienteDTO createDto)
        {
            // Validar RFC único
            var rfcExiste = await _context.Clientes
                .AnyAsync(c => c.Rfc == createDto.Rfc);

            if (rfcExiste)
                throw new InvalidOperationException($"El RFC {createDto.Rfc} ya está registrado");

            var cliente = _mapper.Map<Models.Entities.Cliente>(createDto);
            cliente.FechaRegistro = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            cliente.Activo = true;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente creado: {RazonSocial} - RFC: {Rfc}",
                cliente.NombreRazonSocial, cliente.Rfc);

            return _mapper.Map<ClienteDTO>(cliente);
        }

        public async Task<bool> UpdateByRfcAsync(string rfc, ClienteDTO updateDto)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Rfc == rfc && c.Activo);

            if (cliente == null)
                return false;

            // Si se está cambiando el RFC, validar que no exista otro con el nuevo
            if (!string.IsNullOrEmpty(updateDto.Rfc) && updateDto.Rfc != rfc)
            {
                var rfcExiste = await _context.Clientes
                    .AnyAsync(c => c.Rfc == updateDto.Rfc && c.ClienteId != cliente.ClienteId);

                if (rfcExiste)
                    throw new InvalidOperationException($"El RFC {updateDto.Rfc} ya está registrado");
            }

            // Actualizar propiedades
            cliente.Rfc = updateDto.Rfc;
            cliente.NombreRazonSocial = updateDto.NombreRazonSocial;
            cliente.Telefono = updateDto.Telefono;
            cliente.CorreoElectronico = updateDto.CorreoElectronico;
            cliente.PersonaContacto = updateDto.PersonaContacto;
            cliente.Direccion = updateDto.Direccion;
            cliente.Activo = updateDto.Activo;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente actualizado: {RazonSocial} - RFC: {Rfc}",
                cliente.NombreRazonSocial, cliente.Rfc);

            return true;
        }

        public async Task<bool> DeleteByRfcAsync(string rfc)
        {
            var cliente = await _context.Clientes
                .Include(c => c.OrdenesCarga)
                .FirstOrDefaultAsync(c => c.Rfc == rfc && c.Activo);

            if (cliente == null)
                return false;

            // Verificar si tiene órdenes de carga activas
            var tieneOrdenesActivas = cliente.OrdenesCarga
                .Any(o => o.Estatus != "C" && o.Estatus != "X");

            if (tieneOrdenesActivas)
                throw new InvalidOperationException("No se puede eliminar el cliente porque tiene órdenes de carga activas");

            // Soft delete
            cliente.Activo = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cliente eliminado: {RazonSocial} - RFC: {Rfc}",
                cliente.NombreRazonSocial, cliente.Rfc);

            return true;
        }
    }
}