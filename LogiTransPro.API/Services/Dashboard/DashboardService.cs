using AutoMapper;
using LogiTransPro.API.Data;
using LogiTransPro.API.Models.DTOs.Dashboard;
using LogiTransPro.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiTransPro.API.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly LogiTransProDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(LogiTransProDbContext context, IMapper mapper, ILogger<DashboardService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DashboardDTO> GetDashboardDataAsync()
        {
            var dashboard = new DashboardDTO();

            // Estadísticas de flotilla
            dashboard.TotalVehiculos = await _context.Vehiculos
                .CountAsync(v => v.Activo);

            dashboard.VehiculosEnRuta = await _context.Vehiculos
                .CountAsync(v => v.Activo && v.EstadoGeneral == "R");

            dashboard.VehiculosDisponibles = await _context.Vehiculos
                .CountAsync(v => v.Activo && v.EstadoGeneral == "D");

            dashboard.VehiculosEnMantenimiento = await _context.Vehiculos
                .CountAsync(v => v.Activo && v.EstadoGeneral == "M");

            // Mantenimientos próximos
            dashboard.ProximosMantenimientos = await GetProximosMantenimientosAsync(10);

            // KPIs
            dashboard.KPIs = await GetKPIsAsync();

            // Órdenes pendientes
            dashboard.OrdenesPendientes = await GetOrdenesPendientesCountAsync();

            // Viajes activos
            dashboard.ViajesActivos = await GetViajesActivosCountAsync();

            return dashboard;
        }

        public async Task<KPIDTO> GetKPIsAsync()
        {
            var fechaInicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Unspecified);

            var viajesDelMes = await _context.Viajes
                .Where(v => v.FechaSalidaReal >= fechaInicioMes)
                .ToListAsync();

            var kpi = new KPIDTO();

            kpi.TotalViajesMes = viajesDelMes.Count;

            // Tasa de cumplimiento
            var viajesATiempo = viajesDelMes
                .Count(v => v.FechaLlegadaReal <= v.FechaLlegadaProgramada);

            kpi.EntregasATiempo = viajesATiempo;
            kpi.EntregasTarde = kpi.TotalViajesMes - viajesATiempo;
            kpi.TasaCumplimiento = kpi.TotalViajesMes > 0
                ? (decimal)viajesATiempo / kpi.TotalViajesMes * 100
                : 0;

            // Consumo promedio
            var viajesConConsumo = viajesDelMes
                .Where(v => v.ConsumoCombustible.HasValue && v.KilometrajeFinal.HasValue && v.KilometrajeInicial.HasValue)
                .ToList();

            if (viajesConConsumo.Any())
            {
                var consumoTotal = viajesConConsumo.Sum(v => v.ConsumoCombustible ?? 0);
                var kilometrosTotales = viajesConConsumo.Sum(v => (v.KilometrajeFinal ?? 0) - (v.KilometrajeInicial ?? 0));
                kpi.ConsumoPromedioKM = kilometrosTotales > 0 ? consumoTotal / kilometrosTotales * 100 : 0;
            }

            // Kilómetros recorridos en el mes
            kpi.KilometrosRecorridosMes = viajesDelMes
                .Sum(v => (v.KilometrajeFinal ?? 0) - (v.KilometrajeInicial ?? 0));

            // Costo promedio por viaje
            var viajesConCosto = viajesDelMes
                .Where(v => v.ConsumoCombustible.HasValue)
                .ToList();

            if (viajesConCosto.Any())
            {
                var costoCombustiblePorLitro = 24m;
                var costoTotalCombustible = viajesConCosto.Sum(v => (v.ConsumoCombustible ?? 0) * costoCombustiblePorLitro);
                kpi.CostoPromedioPorViaje = viajesConCosto.Any() ? costoTotalCombustible / viajesConCosto.Count : 0;
            }

            return kpi;
        }

        public async Task<List<ProximoMantenimientoDTO>> GetProximosMantenimientosAsync(int top = 10)
        {
            // Convertir fechas a Unspecified para que coincidan con PostgreSQL
            var fechaActual = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            var fechaLimite = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(30), DateTimeKind.Unspecified);

            var mantenimientos = await _context.Mantenimientos
                .Include(m => m.Vehiculo)
                .Where(m => m.Estatus == "P" &&
                            m.FechaProgramada <= fechaLimite &&
                            m.FechaProgramada >= fechaActual)
                .OrderBy(m => m.FechaProgramada)
                .Take(top)
                .Select(m => new ProximoMantenimientoDTO
                {
                    Placa = m.Vehiculo != null ? m.Vehiculo.Placa : string.Empty,
                    Marca = m.Vehiculo != null ? m.Vehiculo.Marca : string.Empty,
                    Modelo = m.Vehiculo != null ? m.Vehiculo.Modelo : string.Empty,
                    FechaProgramada = m.FechaProgramada,
                    Descripcion = m.Descripcion ?? string.Empty,
                    Prioridad = m.Prioridad,
                    DiasRestantes = (int)Math.Ceiling((m.FechaProgramada - fechaActual).TotalDays)
                })
                .ToListAsync();

            return mantenimientos;
        }

        public async Task<int> GetOrdenesPendientesCountAsync()
        {
            var fechaActual = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            return await _context.OrdenesCarga
                .CountAsync(o => o.Estatus == "P" && o.FechaRequerida >= fechaActual);
        }

        public async Task<int> GetViajesActivosCountAsync()
        {
            return await _context.Viajes
                .CountAsync(v => v.Estatus == "R");
        }
    }
}