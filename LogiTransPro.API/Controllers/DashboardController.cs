using LogiTransPro.API.Attributes;
using LogiTransPro.API.Models.DTOs.Dashboard;
using LogiTransPro.API.Models.ViewModels;
using LogiTransPro.API.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace LogiTransPro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRole]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        /// <summary>
        /// Obtener datos completos del dashboard
        /// </summary>
        [HttpGet]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<DashboardDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard()
        {
            var dashboard = await _dashboardService.GetDashboardDataAsync();
            return Ok(ApiResponse<DashboardDTO>.Ok(dashboard));
        }

        /// <summary>
        /// Obtener KPIs de rendimiento
        /// </summary>
        [HttpGet("kpis")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<KPIDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetKPIs()
        {
            var kpis = await _dashboardService.GetKPIsAsync();
            return Ok(ApiResponse<KPIDTO>.Ok(kpis));
        }

        /// <summary>
        /// Obtener próximos mantenimientos
        /// </summary>
        [HttpGet("mantenimientos-proximos")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<ProximoMantenimientoDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProximosMantenimientos([FromQuery] int top = 10)
        {
            var mantenimientos = await _dashboardService.GetProximosMantenimientosAsync(top);
            return Ok(ApiResponse<List<ProximoMantenimientoDTO>>.Ok(mantenimientos));
        }

        /// <summary>
        /// Obtener resumen rápido del dashboard (versión ligera)
        /// </summary>
        [HttpGet("resumen")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetResumen()
        {
            var dashboard = await _dashboardService.GetDashboardDataAsync();
            var resumen = new
            {
                dashboard.TotalVehiculos,
                dashboard.VehiculosEnRuta,
                dashboard.VehiculosDisponibles,
                dashboard.VehiculosEnMantenimiento,
                dashboard.OrdenesPendientes,
                dashboard.ViajesActivos,
                dashboard.KPIs.TasaCumplimiento,
                dashboard.KPIs.ConsumoPromedioKM
            };
            return Ok(ApiResponse<object>.Ok(resumen));
        }
    }
}