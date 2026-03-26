using AutoMapper;
using LogiTransPro.API.Data;
using LogiTransPro.API.Models.DTOs.Mantenimiento;
using LogiTransPro.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiTransPro.API.Services.Mantenimiento
{
    public class MantenimientoService : IMantenimientoService
    {
        private readonly LogiTransProDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<MantenimientoService> _logger;

        public MantenimientoService(LogiTransProDbContext context, IMapper mapper, ILogger<MantenimientoService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        public async Task<List<MantenimientoDTO>> GetAllAsync()
        {
            var mantenimientos = await _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .Include(m => m.Partes)
                .OrderByDescending(m => m.FechaProgramada)
                .ToListAsync();

            return _mapper.Map<List<MantenimientoDTO>>(mantenimientos);
        }

        public async Task<List<MantenimientoDTO>> GetPendientesAsync()
        {
            var fechaActual = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            var mantenimientos = await _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .Include(m => m.Partes)
                .Where(m => m.Estatus == "P" && m.FechaProgramada >= fechaActual)
                .OrderBy(m => m.FechaProgramada)
                .ToListAsync();

            return _mapper.Map<List<MantenimientoDTO>>(mantenimientos);
        }

        public async Task<List<MantenimientoDTO>> GetProximosAsync(int dias = 30)
        {
            var fechaActual = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            var fechaLimite = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(dias), DateTimeKind.Unspecified);

            var mantenimientos = await _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .Include(m => m.Partes)
                .Where(m => m.Estatus == "P" &&
                            m.FechaProgramada <= fechaLimite &&
                            m.FechaProgramada >= fechaActual)
                .OrderBy(m => m.FechaProgramada)
                .ToListAsync();

            return _mapper.Map<List<MantenimientoDTO>>(mantenimientos);
        }

        // ======================================================
        // BÚSQUEDA POR PLACA DEL VEHÍCULO
        // ======================================================

        public async Task<List<MantenimientoDTO>> GetByPlacaVehiculoAsync(string placa)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);

            if (vehiculo == null)
                throw new KeyNotFoundException($"Vehículo con placa {placa} no encontrado");

            var mantenimientos = await _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .Include(m => m.Partes)
                .Where(m => m.VehiculoId == vehiculo.VehiculoId)
                .OrderByDescending(m => m.FechaProgramada)
                .ToListAsync();

            return _mapper.Map<List<MantenimientoDTO>>(mantenimientos);
        }

        // ======================================================
        // CRUD USANDO PLACA DEL VEHÍCULO
        // ======================================================

        public async Task<MantenimientoDTO> CreateAsync(CrearMantenimientoDTO createDto)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(createDto.VehiculoId);
            if (vehiculo == null)
                throw new KeyNotFoundException($"Vehículo con ID {createDto.VehiculoId} no encontrado");

            var fechaProgramada = DateTime.SpecifyKind(createDto.FechaProgramada, DateTimeKind.Unspecified);
            var fechaRegistro = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            var mantenimiento = new Models.Entities.Mantenimiento
            {
                VehiculoId = createDto.VehiculoId,
                TipoServicio = createDto.TipoServicio,
                Descripcion = createDto.Descripcion,
                FechaProgramada = fechaProgramada,
                KilometrajeAlerta = createDto.KilometrajeAlerta,
                Costo = createDto.Costo,
                Prioridad = createDto.Prioridad,
                TecnicoAsignado = createDto.TecnicoAsignado,
                Estatus = "P",
                FechaRegistro = fechaRegistro,
                KilometrajeActual = vehiculo.KilometrajeActual
            };

            _context.Mantenimientos.Add(mantenimiento);
            await _context.SaveChangesAsync();

            // Agregar partes
            if (createDto.Partes != null && createDto.Partes.Any())
            {
                foreach (var parteDto in createDto.Partes)
                {
                    var parte = new ParteMantenimiento
                    {
                        MantenimientoId = mantenimiento.MantenimientoId,
                        NombreParte = parteDto.NombreParte,
                        Cantidad = parteDto.Cantidad,
                        CostoUnitario = parteDto.CostoUnitario
                    };
                    _context.PartesMantenimiento.Add(parte);
                }
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Mantenimiento creado para vehículo con placa {Placa} - Fecha: {Fecha}",
                vehiculo.Placa, createDto.FechaProgramada);

            return await GetByIdAsync(mantenimiento.MantenimientoId) ?? throw new Exception("Error al obtener el mantenimiento creado");
        }

        public async Task<bool> CompletarByPlacaAsync(string placa, int kilometrajeActual, decimal? costoReal, string notasMecanico)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);

            if (vehiculo == null)
                throw new KeyNotFoundException($"Vehículo con placa {placa} no encontrado");

            // Buscar el mantenimiento pendiente más reciente del vehículo
            var mantenimiento = await _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .Where(m => m.VehiculoId == vehiculo.VehiculoId && m.Estatus == "P")
                .OrderByDescending(m => m.FechaProgramada)
                .FirstOrDefaultAsync();

            if (mantenimiento == null)
                return false;

            var fechaRealizada = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            mantenimiento.Estatus = "C";
            mantenimiento.FechaRealizada = fechaRealizada;
            mantenimiento.KilometrajeActual = kilometrajeActual;
            mantenimiento.NotasMecanico = notasMecanico;
            if (costoReal.HasValue)
                mantenimiento.Costo = costoReal.Value;

            // Si el vehículo estaba en mantenimiento, cambiarlo a disponible
            if (mantenimiento.Vehiculo?.EstadoGeneral == "M")
            {
                mantenimiento.Vehiculo.EstadoGeneral = "D";
                _context.Vehiculos.Update(mantenimiento.Vehiculo);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Mantenimiento completado para vehículo con placa {Placa}", placa);
            return true;
        }

        public async Task<bool> CancelarByPlacaAsync(string placa, string motivo)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);

            if (vehiculo == null)
                throw new KeyNotFoundException($"Vehículo con placa {placa} no encontrado");

            // Buscar el mantenimiento pendiente más reciente del vehículo
            var mantenimiento = await _context.Mantenimientos
                .Where(m => m.VehiculoId == vehiculo.VehiculoId && m.Estatus == "P")
                .OrderByDescending(m => m.FechaProgramada)
                .FirstOrDefaultAsync();

            if (mantenimiento == null)
                return false;

            mantenimiento.Estatus = "X";
            mantenimiento.NotasMecanico = $"Cancelado: {motivo}";

            await _context.SaveChangesAsync();

            _logger.LogInformation("Mantenimiento cancelado para vehículo con placa {Placa}: {Motivo}", placa, motivo);
            return true;
        }

        // ======================================================
        // MÉTODO AUXILIAR (mantenido por compatibilidad)
        // ======================================================

        private async Task<MantenimientoDTO?> GetByIdAsync(int id)
        {
            var mantenimiento = await _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .Include(m => m.Partes)
                .FirstOrDefaultAsync(m => m.MantenimientoId == id);

            return mantenimiento != null ? _mapper.Map<MantenimientoDTO>(mantenimiento) : null;
        }
    }
}