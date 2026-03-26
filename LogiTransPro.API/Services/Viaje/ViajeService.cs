using AutoMapper;
using LogiTransPro.API.Data;
using LogiTransPro.API.Models.DTOs.Viaje;
using LogiTransPro.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiTransPro.API.Services.Viaje
{
    public class ViajeService : IViajeService
    {
        private readonly LogiTransProDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ViajeService> _logger;

        public ViajeService(LogiTransProDbContext context, IMapper mapper, ILogger<ViajeService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        public async Task<List<ViajeDTO>> GetAllAsync()
        {
            var viajes = await _context.Viajes
                .Include(v => v.OrdenCarga)
                .Include(v => v.Vehiculo)
                .Include(v => v.Chofer)
                .Include(v => v.Ruta)
                .OrderByDescending(v => v.FechaRegistro)
                .ToListAsync();

            return _mapper.Map<List<ViajeDTO>>(viajes);
        }

        public async Task<List<ViajeDTO>> GetActivosAsync()
        {
            var viajes = await _context.Viajes
                .Include(v => v.OrdenCarga)
                .Include(v => v.Vehiculo)
                .Include(v => v.Chofer)
                .Include(v => v.Ruta)
                .Where(v => v.Estatus == "R")
                .OrderBy(v => v.FechaSalidaReal)
                .ToListAsync();

            return _mapper.Map<List<ViajeDTO>>(viajes);
        }

        public async Task<List<ViajeDTO>> GetProximosAsync(int dias = 7)
        {
            var fechaActual = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            var fechaLimite = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(dias), DateTimeKind.Unspecified);

            var viajes = await _context.Viajes
                .Include(v => v.OrdenCarga)
                .Include(v => v.Vehiculo)
                .Include(v => v.Chofer)
                .Include(v => v.Ruta)
                .Where(v => v.Estatus == "P" &&
                            v.FechaSalidaProgramada <= fechaLimite &&
                            v.FechaSalidaProgramada >= fechaActual)
                .OrderBy(v => v.FechaSalidaProgramada)
                .ToListAsync();

            return _mapper.Map<List<ViajeDTO>>(viajes);
        }

        // ======================================================
        // BÚSQUEDAS POR NÚMERO DE VIAJE
        // ======================================================

        public async Task<ViajeDTO?> GetByNumeroViajeAsync(string numeroViaje)
        {
            var viaje = await _context.Viajes
                .Include(v => v.OrdenCarga)
                .Include(v => v.Vehiculo)
                .Include(v => v.Chofer)
                .Include(v => v.Ruta)
                .FirstOrDefaultAsync(v => v.NumeroViaje == numeroViaje);

            return viaje != null ? _mapper.Map<ViajeDTO>(viaje) : null;
        }

        public async Task<List<ViajeDTO>> GetByPlacaVehiculoAsync(string placa)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);

            if (vehiculo == null)
                throw new KeyNotFoundException($"Vehículo con placa {placa} no encontrado");

            var viajes = await _context.Viajes
                .Include(v => v.OrdenCarga)
                .Include(v => v.Vehiculo)
                .Include(v => v.Chofer)
                .Include(v => v.Ruta)
                .Where(v => v.VehiculoId == vehiculo.VehiculoId)
                .OrderByDescending(v => v.FechaRegistro)
                .ToListAsync();

            return _mapper.Map<List<ViajeDTO>>(viajes);
        }

        public async Task<List<ViajeDTO>> GetByEmailChoferAsync(string emailChofer)
        {
            var chofer = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CorreoElectronico == emailChofer && u.Estado == "A");

            if (chofer == null)
                throw new KeyNotFoundException($"Chofer con email {emailChofer} no encontrado");

            var viajes = await _context.Viajes
                .Include(v => v.OrdenCarga)
                .Include(v => v.Vehiculo)
                .Include(v => v.Chofer)
                .Include(v => v.Ruta)
                .Where(v => v.ChoferId == chofer.UsuarioId)
                .OrderByDescending(v => v.FechaRegistro)
                .ToListAsync();

            return _mapper.Map<List<ViajeDTO>>(viajes);
        }

        // ======================================================
        // CRUD USANDO NÚMERO DE VIAJE
        // ======================================================

        public async Task<ViajeDTO> CreateAsync(CrearViajeDTO createDto)
        {
            // Validaciones
            var vehiculo = await _context.Vehiculos.FindAsync(createDto.VehiculoId);
            if (vehiculo == null)
                throw new KeyNotFoundException("Vehículo no encontrado");

            if (vehiculo.EstadoGeneral != "D")
                throw new InvalidOperationException("El vehículo no está disponible");

            var chofer = await _context.Usuarios.FindAsync(createDto.ChoferId);
            if (chofer == null)
                throw new KeyNotFoundException("Chofer no encontrado");

            var ordenCarga = await _context.OrdenesCarga.FindAsync(createDto.OrdenCargaId);
            if (ordenCarga == null)
                throw new KeyNotFoundException("Orden de carga no encontrada");

            if (ordenCarga.Estatus != "P")
                throw new InvalidOperationException("La orden de carga no está pendiente");

            var ruta = await _context.Rutas.FindAsync(createDto.RutaId);
            if (ruta == null)
                throw new KeyNotFoundException("Ruta no encontrada");

            // Generar número de viaje
            var numeroViaje = await GenerarNumeroViaje();

            var fechaSalidaProgramada = DateTime.SpecifyKind(createDto.FechaSalidaProgramada, DateTimeKind.Unspecified);
            var fechaLlegadaProgramada = createDto.FechaLlegadaProgramada.HasValue
                ? DateTime.SpecifyKind(createDto.FechaLlegadaProgramada.Value, DateTimeKind.Unspecified)
                : (DateTime?)null;
            var fechaRegistro = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            var viaje = new Models.Entities.Viaje
            {
                NumeroViaje = numeroViaje,
                OrdenCargaId = createDto.OrdenCargaId,
                VehiculoId = createDto.VehiculoId,
                ChoferId = createDto.ChoferId,
                RutaId = createDto.RutaId,
                FechaSalidaProgramada = fechaSalidaProgramada,
                FechaLlegadaProgramada = fechaLlegadaProgramada,
                Observaciones = createDto.Observaciones,
                Estatus = "P",
                FechaRegistro = fechaRegistro
            };

            _context.Viajes.Add(viaje);
            await _context.SaveChangesAsync();

            // Actualizar estado de la orden de carga
            ordenCarga.Estatus = "A"; // Asignada
            await _context.SaveChangesAsync();

            _logger.LogInformation("Viaje creado: {NumeroViaje} - Vehículo: {Placa} - Chofer: {Chofer}",
                viaje.NumeroViaje, vehiculo.Placa, chofer.NombreCompleto);

            return await GetByNumeroViajeAsync(viaje.NumeroViaje) ?? throw new Exception("Error al obtener el viaje creado");
        }

        public async Task<bool> IniciarViajeByNumeroAsync(string numeroViaje, IniciarViajeDTO iniciarDto)
        {
            var viaje = await _context.Viajes
                .Include(v => v.Vehiculo)
                .FirstOrDefaultAsync(v => v.NumeroViaje == numeroViaje);

            if (viaje == null)
                return false;

            if (viaje.Estatus != "P")
                throw new InvalidOperationException("El viaje no está en estado programado");

            var fechaActual = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            if (viaje.FechaSalidaProgramada > fechaActual)
                throw new InvalidOperationException("La fecha de salida programada aún no ha llegado");

            viaje.Estatus = "R";
            viaje.FechaSalidaReal = fechaActual;
            viaje.KilometrajeInicial = iniciarDto.KilometrajeInicial;

            // Actualizar estado del vehículo
            if (viaje.Vehiculo != null)
            {
                viaje.Vehiculo.EstadoGeneral = "R";
                viaje.Vehiculo.KilometrajeActual = iniciarDto.KilometrajeInicial;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Viaje {NumeroViaje} iniciado", viaje.NumeroViaje);
            return true;
        }

        public async Task<bool> FinalizarViajeByNumeroAsync(string numeroViaje, FinalizarViajeDTO finalizarDto)
        {
            var viaje = await _context.Viajes
                .Include(v => v.Vehiculo)
                .FirstOrDefaultAsync(v => v.NumeroViaje == numeroViaje);

            if (viaje == null)
                return false;

            if (viaje.Estatus != "R")
                throw new InvalidOperationException("El viaje no está en ruta");

            if (finalizarDto.KilometrajeFinal < (viaje.KilometrajeInicial ?? 0))
                throw new InvalidOperationException("El kilometraje final no puede ser menor al inicial");

            var fechaActual = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            viaje.Estatus = "C";
            viaje.FechaLlegadaReal = fechaActual;
            viaje.KilometrajeFinal = finalizarDto.KilometrajeFinal;
            viaje.ConsumoCombustible = finalizarDto.ConsumoCombustible;

            // Actualizar estado del vehículo
            if (viaje.Vehiculo != null)
            {
                viaje.Vehiculo.EstadoGeneral = "D";
                viaje.Vehiculo.KilometrajeActual = finalizarDto.KilometrajeFinal;
                viaje.Vehiculo.NivelCombustible -= (finalizarDto.ConsumoCombustible / 100);
            }

            // Actualizar estado de la orden de carga
            var ordenCarga = await _context.OrdenesCarga.FindAsync(viaje.OrdenCargaId);
            if (ordenCarga != null)
            {
                ordenCarga.Estatus = "C";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Viaje {NumeroViaje} finalizado. Km recorridos: {Km}",
                viaje.NumeroViaje, (finalizarDto.KilometrajeFinal - (viaje.KilometrajeInicial ?? 0)));

            return true;
        }

        public async Task<bool> CancelarViajeByNumeroAsync(string numeroViaje, string motivo)
        {
            var viaje = await _context.Viajes
                .Include(v => v.Vehiculo)
                .FirstOrDefaultAsync(v => v.NumeroViaje == numeroViaje);

            if (viaje == null)
                return false;

            if (viaje.Estatus == "C")
                throw new InvalidOperationException("El viaje ya está completado");

            viaje.Estatus = "X";
            viaje.Observaciones = $"Cancelado: {motivo}";

            // Liberar el vehículo si estaba asignado
            if (viaje.Vehiculo != null && viaje.Vehiculo.EstadoGeneral == "R")
            {
                viaje.Vehiculo.EstadoGeneral = "D";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Viaje {NumeroViaje} cancelado: {Motivo}", viaje.NumeroViaje, motivo);
            return true;
        }

        // ======================================================
        // MÉTODO AUXILIAR
        // ======================================================

        private async Task<string> GenerarNumeroViaje()
        {
            var año = DateTime.UtcNow.Year;
            var mes = DateTime.UtcNow.Month.ToString("D2");

            var ultimoViaje = await _context.Viajes
                .Where(v => v.NumeroViaje.StartsWith($"VIA-{año}{mes}"))
                .OrderByDescending(v => v.NumeroViaje)
                .FirstOrDefaultAsync();

            int consecutivo = 1;
            if (ultimoViaje != null)
            {
                var partes = ultimoViaje.NumeroViaje.Split('-');
                if (partes.Length == 3 && int.TryParse(partes[2], out int num))
                {
                    consecutivo = num + 1;
                }
            }

            return $"VIA-{año}{mes}-{consecutivo:D4}";
        }
    }
}