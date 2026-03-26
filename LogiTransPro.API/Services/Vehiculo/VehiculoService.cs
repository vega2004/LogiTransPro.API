using AutoMapper;
using LogiTransPro.API.Data;
using LogiTransPro.API.Models.DTOs.Vehiculo;
using LogiTransPro.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiTransPro.API.Services.Vehiculo
{
    public class VehiculoService : IVehiculoService
    {
        private readonly LogiTransProDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<VehiculoService> _logger;

        public VehiculoService(LogiTransProDbContext context, IMapper mapper, ILogger<VehiculoService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        public async Task<List<VehiculoDTO>> GetAllAsync()
        {
            var vehiculos = await _context.Vehiculos
                .Where(v => v.Activo)
                .OrderBy(v => v.Placa)
                .ToListAsync();

            return _mapper.Map<List<VehiculoDTO>>(vehiculos);
        }

        public async Task<List<VehiculoDTO>> GetDisponiblesAsync()
        {
            var vehiculos = await _context.Vehiculos
                .Where(v => v.Activo && v.EstadoGeneral == "D")
                .OrderBy(v => v.Placa)
                .ToListAsync();

            return _mapper.Map<List<VehiculoDTO>>(vehiculos);
        }

        // ======================================================
        // BÚSQUEDAS POR CAMPOS ÚNICOS DE NEGOCIO
        // ======================================================

        public async Task<VehiculoDTO?> GetByPlacaAsync(string placa)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);

            return vehiculo != null ? _mapper.Map<VehiculoDTO>(vehiculo) : null;
        }

        public async Task<VehiculoDTO?> GetByVinAsync(string vin)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Vin == vin && v.Activo);

            return vehiculo != null ? _mapper.Map<VehiculoDTO>(vehiculo) : null;
        }

        // ======================================================
        // CRUD USANDO PLACA COMO IDENTIFICADOR
        // ======================================================

        public async Task<VehiculoDTO> CreateAsync(CrearVehiculoDTO createDto)
        {
            // Validar placa única
            var placaExiste = await _context.Vehiculos
                .AnyAsync(v => v.Placa == createDto.Placa);

            if (placaExiste)
                throw new InvalidOperationException($"La placa {createDto.Placa} ya está registrada");

            // Validar VIN único
            var vinExiste = await _context.Vehiculos
                .AnyAsync(v => v.Vin == createDto.Vin);

            if (vinExiste)
                throw new InvalidOperationException($"El VIN {createDto.Vin} ya está registrado");

            var vehiculo = _mapper.Map<Models.Entities.Vehiculo>(createDto);
            vehiculo.FechaRegistro = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            vehiculo.Activo = true;

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Vehículo creado: {Placa} - {Marca} {Modelo}",
                vehiculo.Placa, vehiculo.Marca, vehiculo.Modelo);

            return _mapper.Map<VehiculoDTO>(vehiculo);
        }

        public async Task<bool> UpdateByPlacaAsync(string placa, ActualizarVehiculoDTO updateDto)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);

            if (vehiculo == null)
                return false;

            // Si se está cambiando la placa, validar que no exista otra con la nueva
            if (!string.IsNullOrEmpty(updateDto.Placa) && updateDto.Placa != placa)
            {
                var placaExiste = await _context.Vehiculos
                    .AnyAsync(v => v.Placa == updateDto.Placa && v.VehiculoId != vehiculo.VehiculoId);

                if (placaExiste)
                    throw new InvalidOperationException($"La placa {updateDto.Placa} ya está registrada");
            }

            // Si se está cambiando el VIN, validar que no exista otro con el nuevo
            if (!string.IsNullOrEmpty(updateDto.Vin) && updateDto.Vin != vehiculo.Vin)
            {
                var vinExiste = await _context.Vehiculos
                    .AnyAsync(v => v.Vin == updateDto.Vin && v.VehiculoId != vehiculo.VehiculoId);

                if (vinExiste)
                    throw new InvalidOperationException($"El VIN {updateDto.Vin} ya está registrado");
            }

            _mapper.Map(updateDto, vehiculo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Vehículo actualizado: {Placa}", vehiculo.Placa);
            return true;
        }

        public async Task<bool> DeleteByPlacaAsync(string placa)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);

            if (vehiculo == null)
                return false;

            // Soft delete
            vehiculo.Activo = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Vehículo eliminado (soft delete): {Placa}", vehiculo.Placa);
            return true;
        }

        // ======================================================
        // OPERACIONES ESPECÍFICAS USANDO PLACA
        // ======================================================

        public async Task<bool> ActualizarKilometrajeByPlacaAsync(string placa, int kilometraje)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);

            if (vehiculo == null)
                return false;

            if (kilometraje < vehiculo.KilometrajeActual)
                throw new InvalidOperationException("El kilometraje no puede ser menor al actual");

            vehiculo.KilometrajeActual = kilometraje;
            await _context.SaveChangesAsync();

            // Verificar si requiere mantenimiento
            await VerificarMantenimientoPreventivo(vehiculo);

            return true;
        }

        public async Task<bool> ActualizarCombustibleByPlacaAsync(string placa, decimal nivelCombustible)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);

            if (vehiculo == null)
                return false;

            if (nivelCombustible < 0 || nivelCombustible > 100)
                throw new InvalidOperationException("El nivel de combustible debe estar entre 0 y 100");

            vehiculo.NivelCombustible = nivelCombustible;
            await _context.SaveChangesAsync();

            return true;
        }

        // ======================================================
        // MÉTODOS OBSOLETOS (mantener por compatibilidad)
        // ======================================================

        [Obsolete("Usar GetByPlacaAsync en su lugar")]
        public async Task<VehiculoDTO?> GetByIdAsync(int id)
        {
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.VehiculoId == id && v.Activo);

            return vehiculo != null ? _mapper.Map<VehiculoDTO>(vehiculo) : null;
        }

        [Obsolete("Usar UpdateByPlacaAsync en su lugar")]
        public async Task<bool> UpdateAsync(int id, ActualizarVehiculoDTO updateDto)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
                return false;

            _mapper.Map(updateDto, vehiculo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Vehículo actualizado: {Placa}", vehiculo.Placa);
            return true;
        }

        [Obsolete("Usar DeleteByPlacaAsync en su lugar")]
        public async Task<bool> DeleteAsync(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
                return false;

            vehiculo.Activo = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Vehículo eliminado (soft delete): {Placa}", vehiculo.Placa);
            return true;
        }

        [Obsolete("Usar ActualizarKilometrajeByPlacaAsync en su lugar")]
        public async Task<bool> ActualizarKilometrajeAsync(int id, int kilometraje)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
                return false;

            if (kilometraje < vehiculo.KilometrajeActual)
                throw new InvalidOperationException("El kilometraje no puede ser menor al actual");

            vehiculo.KilometrajeActual = kilometraje;
            await _context.SaveChangesAsync();

            await VerificarMantenimientoPreventivo(vehiculo);

            return true;
        }

        [Obsolete("Usar ActualizarCombustibleByPlacaAsync en su lugar")]
        public async Task<bool> ActualizarCombustibleAsync(int id, decimal nivelCombustible)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null)
                return false;

            if (nivelCombustible < 0 || nivelCombustible > 100)
                throw new InvalidOperationException("El nivel de combustible debe estar entre 0 y 100");

            vehiculo.NivelCombustible = nivelCombustible;
            await _context.SaveChangesAsync();

            return true;
        }

        // ======================================================
        // MÉTODO PRIVADO
        // ======================================================

        private async Task VerificarMantenimientoPreventivo(Models.Entities.Vehiculo vehiculo)
        {
            var mantenimientosPendientes = await _context.Mantenimientos
                .Where(m => m.VehiculoId == vehiculo.VehiculoId &&
                            m.TipoServicio == "P" &&
                            m.Estatus == "P" &&
                            m.KilometrajeAlerta.HasValue &&
                            m.KilometrajeAlerta <= vehiculo.KilometrajeActual)
                .ToListAsync();

            foreach (var mantenimiento in mantenimientosPendientes)
            {
                mantenimiento.Estatus = "E";
                _logger.LogWarning("Mantenimiento programado para vehículo {Placa} - Kilometraje alcanzado: {Kilometraje}",
                    vehiculo.Placa, vehiculo.KilometrajeActual);
            }

            if (mantenimientosPendientes.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
    }
}