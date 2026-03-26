using LogiTransPro.API.Attributes;
using LogiTransPro.API.Models.DTOs.Mantenimiento;
using LogiTransPro.API.Models.ViewModels;
using LogiTransPro.API.Services.Mantenimiento;
using Microsoft.AspNetCore.Mvc;

namespace LogiTransPro.API.Controllers
{
    [ApiController]
    [Route("api/mantenimiento")]
    [AuthorizeRole]
    public class MantenimientoController : ControllerBase
    {
        private readonly IMantenimientoService _mantenimientoService;
        private readonly ILogger<MantenimientoController> _logger;

        public MantenimientoController(IMantenimientoService mantenimientoService, ILogger<MantenimientoController> logger)
        {
            _mantenimientoService = mantenimientoService;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        /// <summary>
        /// GET /api/mantenimiento - Obtener todos los mantenimientos
        /// </summary>
        [HttpGet]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<MantenimientoDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var mantenimientos = await _mantenimientoService.GetAllAsync();
            return Ok(ApiResponse<List<MantenimientoDTO>>.Ok(mantenimientos));
        }

        /// <summary>
        /// GET /api/mantenimiento/proximos - Obtener mantenimientos próximos
        /// </summary>
        [HttpGet("proximos")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<MantenimientoDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProximos([FromQuery] int dias = 30)
        {
            var mantenimientos = await _mantenimientoService.GetProximosAsync(dias);
            return Ok(ApiResponse<List<MantenimientoDTO>>.Ok(mantenimientos));
        }

        /// <summary>
        /// GET /api/mantenimiento/pendientes - Obtener mantenimientos pendientes
        /// </summary>
        [HttpGet("pendientes")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<MantenimientoDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendientes()
        {
            var mantenimientos = await _mantenimientoService.GetPendientesAsync();
            return Ok(ApiResponse<List<MantenimientoDTO>>.Ok(mantenimientos));
        }

        // ======================================================
        // BÚSQUEDA POR PLACA DEL VEHÍCULO
        // ======================================================

        /// <summary>
        /// GET /api/mantenimiento/vehiculo/{placa} - Obtener mantenimientos por placa del vehículo
        /// </summary>
        [HttpGet("vehiculo/{placa}")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<MantenimientoDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByPlacaVehiculo(string placa)
        {
            try
            {
                var mantenimientos = await _mantenimientoService.GetByPlacaVehiculoAsync(placa);
                return Ok(ApiResponse<List<MantenimientoDTO>>.Ok(mantenimientos));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Error(ex.Message));
            }
        }

        // ======================================================
        // CRUD USANDO PLACA DEL VEHÍCULO
        // ======================================================
        /// <summary>
        /// POST /api/mantenimiento - Crear nuevo mantenimiento (Solo Admin y Supervisor)
        /// </summary>
        [HttpPost]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<MantenimientoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CrearMantenimientoDTO createDto)
        {
            try
            {
                var mantenimiento = await _mantenimientoService.CreateAsync(createDto);
                return Ok(ApiResponse<MantenimientoDTO>.Ok(mantenimiento, "Mantenimiento creado exitosamente"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Error(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// PATCH /api/mantenimiento/vehiculo/{placa}/completar - Completar mantenimiento por placa
        /// </summary>
        [HttpPatch("vehiculo/{placa}/completar")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompletarByPlaca(string placa, [FromBody] CompletarMantenimientoDTO completarDto)
        {
            try
            {
                var result = await _mantenimientoService.CompletarByPlacaAsync(
                    placa,
                    completarDto.KilometrajeActual,
                    completarDto.CostoReal,
                    completarDto.NotasMecanico);

                if (!result)
                    return NotFound(ApiResponse<object>.Error($"No se encontró mantenimiento pendiente para vehículo con placa {placa}"));

                return Ok(ApiResponse<bool>.Ok(true, "Mantenimiento completado exitosamente"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// PATCH /api/mantenimiento/vehiculo/{placa}/cancelar - Cancelar mantenimiento por placa
        /// </summary>
        [HttpPatch("vehiculo/{placa}/cancelar")]
        [AdminOnly]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelarByPlaca(string placa, [FromBody] CancelarMantenimientoDTO cancelarDto)
        {
            try
            {
                var result = await _mantenimientoService.CancelarByPlacaAsync(placa, cancelarDto.Motivo);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"No se encontró mantenimiento pendiente para vehículo con placa {placa}"));

                return Ok(ApiResponse<bool>.Ok(true, "Mantenimiento cancelado exitosamente"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Error(ex.Message));
            }
        }
    }

    public class CompletarMantenimientoDTO
    {
        public int KilometrajeActual { get; set; }
        public decimal? CostoReal { get; set; }
        public string NotasMecanico { get; set; } = string.Empty;
    }

    public class CancelarMantenimientoDTO
    {
        public string Motivo { get; set; } = string.Empty;
    }
}