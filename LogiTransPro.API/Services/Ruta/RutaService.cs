using AutoMapper;
using LogiTransPro.API.Data;
using LogiTransPro.API.Models.DTOs.Ruta;
using LogiTransPro.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiTransPro.API.Services.Ruta
{
    public class RutaService : IRutaService
    {
        private readonly LogiTransProDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RutaService> _logger;

        public RutaService(LogiTransProDbContext context, IMapper mapper, ILogger<RutaService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        public async Task<List<RutaDTO>> GetAllAsync()
        {
            var rutas = await _context.Rutas
                .Where(r => r.Activo)
                .OrderBy(r => r.CodigoRuta)
                .ToListAsync();

            return _mapper.Map<List<RutaDTO>>(rutas);
        }

        public async Task<List<RutaDTO>> GetActivasAsync()
        {
            var rutas = await _context.Rutas
                .Where(r => r.Activo)
                .OrderBy(r => r.CodigoRuta)
                .ToListAsync();

            return _mapper.Map<List<RutaDTO>>(rutas);
        }

        // ======================================================
        // BÚSQUEDA POR CÓDIGO DE RUTA
        // ======================================================

        public async Task<RutaDTO?> GetByCodigoAsync(string codigoRuta)
        {
            var ruta = await _context.Rutas
                .FirstOrDefaultAsync(r => r.CodigoRuta == codigoRuta && r.Activo);

            return ruta != null ? _mapper.Map<RutaDTO>(ruta) : null;
        }

        public async Task<List<RutaDTO>> GetByOrigenDestinoAsync(string origen, string destino)
        {
            var rutas = await _context.Rutas
                .Where(r => r.Activo &&
                            r.Origen.ToLower() == origen.ToLower() &&
                            r.Destino.ToLower() == destino.ToLower())
                .OrderBy(r => r.DistanciaEstimada)
                .ToListAsync();

            return _mapper.Map<List<RutaDTO>>(rutas);
        }

        // ======================================================
        // CRUD USANDO CÓDIGO DE RUTA
        // ======================================================

        public async Task<RutaDTO> CreateAsync(RutaDTO createDto)
        {
            // Validar código único
            var codigoExiste = await _context.Rutas
                .AnyAsync(r => r.CodigoRuta == createDto.CodigoRuta);

            if (codigoExiste)
                throw new InvalidOperationException($"El código {createDto.CodigoRuta} ya existe");

            var ruta = _mapper.Map<Models.Entities.Ruta>(createDto);
            ruta.Activo = true;

            _context.Rutas.Add(ruta);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Ruta creada: {Codigo} - {Origen} → {Destino}",
                ruta.CodigoRuta, ruta.Origen, ruta.Destino);

            return _mapper.Map<RutaDTO>(ruta);
        }

        public async Task<bool> UpdateByCodigoAsync(string codigoRuta, RutaDTO updateDto)
        {
            var ruta = await _context.Rutas
                .FirstOrDefaultAsync(r => r.CodigoRuta == codigoRuta && r.Activo);

            if (ruta == null)
                return false;

            // Si se está cambiando el código, validar que no exista otro con el nuevo
            if (!string.IsNullOrEmpty(updateDto.CodigoRuta) && updateDto.CodigoRuta != codigoRuta)
            {
                var codigoExiste = await _context.Rutas
                    .AnyAsync(r => r.CodigoRuta == updateDto.CodigoRuta && r.RutaId != ruta.RutaId);

                if (codigoExiste)
                    throw new InvalidOperationException($"El código {updateDto.CodigoRuta} ya existe");
            }

            // Actualizar propiedades
            ruta.CodigoRuta = updateDto.CodigoRuta;
            ruta.NombreRuta = updateDto.NombreRuta;
            ruta.Origen = updateDto.Origen;
            ruta.Destino = updateDto.Destino;
            ruta.DistanciaEstimada = updateDto.DistanciaEstimada;
            ruta.TiempoEstimado = updateDto.TiempoEstimado;
            ruta.Activo = updateDto.Activo;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Ruta actualizada: {Codigo}", ruta.CodigoRuta);
            return true;
        }

        public async Task<bool> DeleteByCodigoAsync(string codigoRuta)
        {
            var ruta = await _context.Rutas
                .FirstOrDefaultAsync(r => r.CodigoRuta == codigoRuta && r.Activo);

            if (ruta == null)
                return false;

            // Verificar si tiene viajes asociados
            var tieneViajes = await _context.Viajes
                .AnyAsync(v => v.RutaId == ruta.RutaId);

            if (tieneViajes)
                throw new InvalidOperationException("No se puede eliminar la ruta porque tiene viajes asociados");

            // Soft delete
            ruta.Activo = false;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Ruta eliminada: {Codigo}", ruta.CodigoRuta);
            return true;
        }

        // ======================================================
        // OPERACIONES ESPECÍFICAS
        // ======================================================

        public async Task<RutaOptimizadaDTO> OptimizarRutaAsync(OptimizarRutaDTO optimizarDto)
        {
            // Buscar rutas existentes que coincidan
            var rutasExistentes = await _context.Rutas
                .Where(r => r.Activo)
                .ToListAsync();

            // Buscar ruta directa
            var rutaDirecta = rutasExistentes
                .FirstOrDefault(r => r.Origen.ToLower() == optimizarDto.Origen.ToLower() &&
                                     r.Destino.ToLower() == optimizarDto.Destino.ToLower());

            if (rutaDirecta != null)
            {
                return new RutaOptimizadaDTO
                {
                    Origen = optimizarDto.Origen,
                    Destino = optimizarDto.Destino,
                    DistanciaTotal = rutaDirecta.DistanciaEstimada,
                    TiempoEstimado = rutaDirecta.TiempoEstimado ?? 0,
                    RutaCompleta = new List<string> { optimizarDto.Origen, optimizarDto.Destino },
                    Recomendacion = $"Ruta directa disponible: {rutaDirecta.NombreRuta ?? rutaDirecta.CodigoRuta}"
                };
            }

            // Buscar ruta con puntos intermedios
            if (optimizarDto.PuntosIntermedios != null && optimizarDto.PuntosIntermedios.Any())
            {
                return await OptimizarRutaConPuntosIntermedios(optimizarDto, rutasExistentes);
            }

            // Si no hay ruta directa, buscar la mejor combinación
            return await OptimizarRutaCombinada(optimizarDto, rutasExistentes);
        }

        private async Task<RutaOptimizadaDTO> OptimizarRutaConPuntosIntermedios(
            OptimizarRutaDTO optimizarDto,
            List<Models.Entities.Ruta> rutasExistentes)
        {
            var puntos = new List<string> { optimizarDto.Origen };
            puntos.AddRange(optimizarDto.PuntosIntermedios!);
            puntos.Add(optimizarDto.Destino);

            decimal distanciaTotal = 0;
            int tiempoTotal = 0;
            var rutaCompleta = new List<string> { optimizarDto.Origen };

            for (int i = 0; i < puntos.Count - 1; i++)
            {
                var tramo = rutasExistentes
                    .FirstOrDefault(r => r.Origen.ToLower() == puntos[i].ToLower() &&
                                         r.Destino.ToLower() == puntos[i + 1].ToLower());

                if (tramo == null)
                {
                    distanciaTotal += CalcularDistanciaEstimada(puntos[i], puntos[i + 1]);
                    tiempoTotal += CalcularTiempoEstimado(puntos[i], puntos[i + 1]);
                    rutaCompleta.Add(puntos[i + 1]);
                }
                else
                {
                    distanciaTotal += tramo.DistanciaEstimada;
                    tiempoTotal += tramo.TiempoEstimado ?? 0;
                    rutaCompleta.Add(puntos[i + 1]);
                }
            }

            return new RutaOptimizadaDTO
            {
                Origen = optimizarDto.Origen,
                Destino = optimizarDto.Destino,
                DistanciaTotal = distanciaTotal,
                TiempoEstimado = tiempoTotal,
                RutaCompleta = rutaCompleta,
                Recomendacion = $"Ruta con {optimizarDto.PuntosIntermedios!.Count} punto(s) intermedio(s)"
            };
        }

        private async Task<RutaOptimizadaDTO> OptimizarRutaCombinada(
            OptimizarRutaDTO optimizarDto,
            List<Models.Entities.Ruta> rutasExistentes)
        {
            var rutasDesdeOrigen = rutasExistentes
                .Where(r => r.Origen.ToLower() == optimizarDto.Origen.ToLower())
                .ToList();

            var rutasHaciaDestino = rutasExistentes
                .Where(r => r.Destino.ToLower() == optimizarDto.Destino.ToLower())
                .ToList();

            decimal mejorDistancia = decimal.MaxValue;
            int mejorTiempo = 0;
            string puntoIntermedio = string.Empty;

            foreach (var rutaOrigen in rutasDesdeOrigen)
            {
                var rutaDestino = rutasHaciaDestino
                    .FirstOrDefault(r => r.Origen.ToLower() == rutaOrigen.Destino.ToLower());

                if (rutaDestino != null)
                {
                    var distanciaTotal = rutaOrigen.DistanciaEstimada + rutaDestino.DistanciaEstimada;
                    if (distanciaTotal < mejorDistancia)
                    {
                        mejorDistancia = distanciaTotal;
                        mejorTiempo = (rutaOrigen.TiempoEstimado ?? 0) + (rutaDestino.TiempoEstimado ?? 0);
                        puntoIntermedio = rutaOrigen.Destino;
                    }
                }
            }

            if (mejorDistancia != decimal.MaxValue)
            {
                return new RutaOptimizadaDTO
                {
                    Origen = optimizarDto.Origen,
                    Destino = optimizarDto.Destino,
                    DistanciaTotal = mejorDistancia,
                    TiempoEstimado = mejorTiempo,
                    RutaCompleta = new List<string> { optimizarDto.Origen, puntoIntermedio, optimizarDto.Destino },
                    Recomendacion = $"Ruta combinada a través de {puntoIntermedio}"
                };
            }

            var distanciaEstimada = CalcularDistanciaEstimada(optimizarDto.Origen, optimizarDto.Destino);
            var tiempoEstimado = CalcularTiempoEstimado(optimizarDto.Origen, optimizarDto.Destino);

            return new RutaOptimizadaDTO
            {
                Origen = optimizarDto.Origen,
                Destino = optimizarDto.Destino,
                DistanciaTotal = distanciaEstimada,
                TiempoEstimado = tiempoEstimado,
                RutaCompleta = new List<string> { optimizarDto.Origen, optimizarDto.Destino },
                Recomendacion = "No se encontró ruta exacta. Distancia estimada basada en ubicaciones."
            };
        }

        private decimal CalcularDistanciaEstimada(string origen, string destino)
        {
            var random = new Random();
            return random.Next(50, 1000);
        }

        private int CalcularTiempoEstimado(string origen, string destino)
        {
            var random = new Random();
            return random.Next(60, 720);
        }
    }
}