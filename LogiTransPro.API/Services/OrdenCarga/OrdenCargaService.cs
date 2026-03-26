using AutoMapper;
using LogiTransPro.API.Data;
using LogiTransPro.API.Models.DTOs.OrdenCarga;
using LogiTransPro.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiTransPro.API.Services.OrdenCarga
{
    public class OrdenCargaService : IOrdenCargaService
    {
        private readonly LogiTransProDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdenCargaService> _logger;

        public OrdenCargaService(LogiTransProDbContext context, IMapper mapper, ILogger<OrdenCargaService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        public async Task<List<OrdenCargaDTO>> GetAllAsync()
        {
            var ordenes = await _context.OrdenesCarga
                .Include(o => o.Cliente)
                .OrderByDescending(o => o.FechaSolicitud)
                .ToListAsync();

            return _mapper.Map<List<OrdenCargaDTO>>(ordenes);
        }

        public async Task<List<OrdenCargaDTO>> GetPendientesAsync()
        {
            var fechaActual = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            var ordenes = await _context.OrdenesCarga
                .Include(o => o.Cliente)
                .Where(o => o.Estatus == "P" && o.FechaRequerida >= fechaActual)
                .OrderBy(o => o.Prioridad == "Urgente" ? 0 : o.Prioridad == "Alta" ? 1 : 2)
                .ThenBy(o => o.FechaRequerida)
                .ToListAsync();

            return _mapper.Map<List<OrdenCargaDTO>>(ordenes);
        }

        public async Task<List<OrdenCargaDTO>> GetUrgentesAsync()
        {
            var ordenes = await _context.OrdenesCarga
                .Include(o => o.Cliente)
                .Where(o => o.Prioridad == "Urgente" && o.Estatus == "P")
                .OrderBy(o => o.FechaRequerida)
                .ToListAsync();

            return _mapper.Map<List<OrdenCargaDTO>>(ordenes);
        }

        // ======================================================
        // BÚSQUEDAS POR NÚMERO DE ORDEN
        // ======================================================

        public async Task<OrdenCargaDTO?> GetByNumeroOrdenAsync(string numeroOrden)
        {
            var orden = await _context.OrdenesCarga
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(o => o.NumeroOrden == numeroOrden);

            return orden != null ? _mapper.Map<OrdenCargaDTO>(orden) : null;
        }

        public async Task<List<OrdenCargaDTO>> GetByClienteRfcAsync(string clienteRfc)
        {
            var ordenes = await _context.OrdenesCarga
                .Include(o => o.Cliente)
                .Where(o => o.Cliente != null && o.Cliente.Rfc == clienteRfc)
                .OrderByDescending(o => o.FechaSolicitud)
                .ToListAsync();

            return _mapper.Map<List<OrdenCargaDTO>>(ordenes);
        }

        // ======================================================
        // CRUD USANDO NÚMERO DE ORDEN
        // ======================================================

        public async Task<OrdenCargaDTO> CreateAsync(CrearOrdenCargaDTO createDto)
        {
            var cliente = await _context.Clientes.FindAsync(createDto.ClienteId);
            if (cliente == null)
                throw new KeyNotFoundException("Cliente no encontrado");

            if (!cliente.Activo)
                throw new InvalidOperationException("El cliente no está activo");

            var orden = _mapper.Map<Models.Entities.OrdenCarga>(createDto);
            orden.NumeroOrden = await GenerarNumeroOrden();
            orden.FechaSolicitud = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            orden.Estatus = "P";

            _context.OrdenesCarga.Add(orden);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Orden de carga creada: {NumeroOrden} - Cliente: {Cliente}",
                orden.NumeroOrden, cliente.NombreRazonSocial);

            return await GetByNumeroOrdenAsync(orden.NumeroOrden) ?? throw new Exception("Error al obtener la orden creada");
        }

        public async Task<bool> UpdateByNumeroOrdenAsync(string numeroOrden, OrdenCargaDTO updateDto)
        {
            var orden = await _context.OrdenesCarga
                .FirstOrDefaultAsync(o => o.NumeroOrden == numeroOrden);

            if (orden == null)
                return false;

            if (orden.Estatus != "P")
                throw new InvalidOperationException("Solo se pueden editar órdenes pendientes");

            // Si se está cambiando el número de orden, validar que no exista otro
            if (!string.IsNullOrEmpty(updateDto.NumeroOrden) && updateDto.NumeroOrden != numeroOrden)
            {
                var existe = await _context.OrdenesCarga
                    .AnyAsync(o => o.NumeroOrden == updateDto.NumeroOrden);

                if (existe)
                    throw new InvalidOperationException($"El número de orden {updateDto.NumeroOrden} ya existe");
            }

            // Actualizar propiedades
            orden.NumeroOrden = updateDto.NumeroOrden;
            orden.ClienteId = updateDto.ClienteId;
            orden.FechaRequerida = updateDto.FechaRequerida;
            orden.DescripcionMercancia = updateDto.DescripcionMercancia;
            orden.PesoTotal = updateDto.PesoTotal;
            orden.VolumenTotal = updateDto.VolumenTotal;
            orden.InstruccionesEspeciales = updateDto.InstruccionesEspeciales;
            orden.ValorDeclarado = updateDto.ValorDeclarado;
            orden.Prioridad = updateDto.Prioridad;
            orden.Estatus = updateDto.Estatus;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Orden de carga actualizada: {NumeroOrden}", orden.NumeroOrden);
            return true;
        }

        public async Task<bool> CancelarByNumeroOrdenAsync(string numeroOrden, string motivo)
        {
            var orden = await _context.OrdenesCarga
                .Include(o => o.Viajes)
                .FirstOrDefaultAsync(o => o.NumeroOrden == numeroOrden);

            if (orden == null)
                return false;

            if (orden.Estatus == "C")
                throw new InvalidOperationException("La orden ya está completada");

            if (orden.Viajes.Any(v => v.Estatus == "R"))
                throw new InvalidOperationException("No se puede cancelar la orden porque tiene un viaje en curso");

            orden.Estatus = "X";
            orden.InstruccionesEspeciales = $"Cancelada: {motivo}";

            await _context.SaveChangesAsync();

            _logger.LogInformation("Orden de carga cancelada: {NumeroOrden} - Motivo: {Motivo}",
                orden.NumeroOrden, motivo);

            return true;
        }

        public async Task<bool> AsignarViajeByNumeroOrdenAsync(string numeroOrden, string numeroViaje)
        {
            var orden = await _context.OrdenesCarga
                .FirstOrDefaultAsync(o => o.NumeroOrden == numeroOrden);

            if (orden == null)
                return false;

            var viaje = await _context.Viajes
                .FirstOrDefaultAsync(v => v.NumeroViaje == numeroViaje);

            if (viaje == null)
                return false;

            orden.Estatus = "A"; // Asignada
            await _context.SaveChangesAsync();

            _logger.LogInformation("Viaje {NumeroViaje} asignado a orden {NumeroOrden}", numeroViaje, numeroOrden);
            return true;
        }

        // ======================================================
        // MÉTODO AUXILIAR
        // ======================================================

        private async Task<string> GenerarNumeroOrden()
        {
            var año = DateTime.UtcNow.Year;
            var mes = DateTime.UtcNow.Month.ToString("D2");

            var ultimaOrden = await _context.OrdenesCarga
                .Where(o => o.NumeroOrden.StartsWith($"OC-{año}{mes}"))
                .OrderByDescending(o => o.NumeroOrden)
                .FirstOrDefaultAsync();

            int consecutivo = 1;
            if (ultimaOrden != null)
            {
                var partes = ultimaOrden.NumeroOrden.Split('-');
                if (partes.Length == 3 && int.TryParse(partes[2], out int num))
                {
                    consecutivo = num + 1;
                }
            }

            return $"OC-{año}{mes}-{consecutivo:D4}";
        }
    }
}