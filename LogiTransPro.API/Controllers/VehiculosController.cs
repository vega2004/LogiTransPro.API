using LogiTransPro.API.Attributes;
using LogiTransPro.API.Models.DTOs.Vehiculo;
using LogiTransPro.API.Models.ViewModels;
using LogiTransPro.API.Services.Vehiculo;
using Microsoft.AspNetCore.Mvc;

namespace LogiTransPro.API.Controllers
{
    [ApiController]
    [Route("api/vehiculos")]
    [AuthorizeRole]
    public class VehiculosController : ControllerBase
    {
        private readonly IVehiculoService _vehiculoService;
        private readonly ILogger<VehiculosController> _logger;

        public VehiculosController(IVehiculoService vehiculoService, ILogger<VehiculosController> logger)
        {
            _vehiculoService = vehiculoService;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        /// <summary>
        /// GET /api/vehiculos - Obtener todos los vehículos
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<VehiculoDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var vehiculos = await _vehiculoService.GetAllAsync();
            return Ok(ApiResponse<List<VehiculoDTO>>.Ok(vehiculos));
        }

        /// <summary>
        /// GET /api/vehiculos/disponibles - Obtener vehículos disponibles
        /// </summary>
        [HttpGet("disponibles")]
        [ProducesResponseType(typeof(ApiResponse<List<VehiculoDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDisponibles()
        {
            var vehiculos = await _vehiculoService.GetDisponiblesAsync();
            return Ok(ApiResponse<List<VehiculoDTO>>.Ok(vehiculos));
        }

        // ======================================================
        // BÚSQUEDAS POR CAMPOS ÚNICOS
        // ======================================================

        /// <summary>
        /// GET /api/vehiculos/placa/{placa} - Obtener vehículo por placa
        /// </summary>
        [HttpGet("placa/{placa}")]
        [ProducesResponseType(typeof(ApiResponse<VehiculoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByPlaca(string placa)
        {
            var vehiculo = await _vehiculoService.GetByPlacaAsync(placa);
            if (vehiculo == null)
                return NotFound(ApiResponse<object>.Error($"Vehículo con placa {placa} no encontrado"));

            return Ok(ApiResponse<VehiculoDTO>.Ok(vehiculo));
        }

        /// <summary>
        /// GET /api/vehiculos/vin/{vin} - Obtener vehículo por VIN
        /// </summary>
        [HttpGet("vin/{vin}")]
        [ProducesResponseType(typeof(ApiResponse<VehiculoDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByVin(string vin)
        {
            var vehiculo = await _vehiculoService.GetByVinAsync(vin);
            if (vehiculo == null)
                return NotFound(ApiResponse<object>.Error($"Vehículo con VIN {vin} no encontrado"));

            return Ok(ApiResponse<VehiculoDTO>.Ok(vehiculo));
        }

        // ======================================================
        // CRUD
        // ======================================================

        /// <summary>
        /// POST /api/vehiculos - Crear nuevo vehículo (Solo Admin)
        /// </summary>
        [HttpPost]
        [AdminOnly]
        [ProducesResponseType(typeof(ApiResponse<VehiculoDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CrearVehiculoDTO createDto)
        {
            try
            {
                var vehiculo = await _vehiculoService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetByPlaca), new { placa = vehiculo.Placa },
                    ApiResponse<VehiculoDTO>.Ok(vehiculo, "Vehículo creado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// PUT /api/vehiculos/placa/{placa} - Actualizar vehículo por placa (Solo Admin)
        /// </summary>
        [HttpPut("placa/{placa}")]
        [AdminOnly]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateByPlaca(string placa, [FromBody] ActualizarVehiculoDTO updateDto)
        {
            try
            {
                var result = await _vehiculoService.UpdateByPlacaAsync(placa, updateDto);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Vehículo con placa {placa} no encontrado"));

                return Ok(ApiResponse<bool>.Ok(true, "Vehículo actualizado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// DELETE /api/vehiculos/placa/{placa} - Eliminar vehículo por placa (Soft delete - Solo Admin)
        /// </summary>
        [HttpDelete("placa/{placa}")]
        [AdminOnly]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteByPlaca(string placa)
        {
            var result = await _vehiculoService.DeleteByPlacaAsync(placa);
            if (!result)
                return NotFound(ApiResponse<object>.Error($"Vehículo con placa {placa} no encontrado"));

            return Ok(ApiResponse<bool>.Ok(true, "Vehículo eliminado exitosamente"));
        }

        // ======================================================
        // OPERACIONES ESPECÍFICAS
        // ======================================================

        /// <summary>
        /// PATCH /api/vehiculos/placa/{placa}/kilometraje - Actualizar kilometraje del vehículo por placa
        /// </summary>
        [HttpPatch("placa/{placa}/kilometraje")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActualizarKilometraje(string placa, [FromBody] int kilometraje)
        {
            try
            {
                var result = await _vehiculoService.ActualizarKilometrajeByPlacaAsync(placa, kilometraje);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Vehículo con placa {placa} no encontrado"));

                return Ok(ApiResponse<bool>.Ok(true, "Kilometraje actualizado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// PATCH /api/vehiculos/placa/{placa}/combustible - Actualizar nivel de combustible por placa
        /// </summary>
        [HttpPatch("placa/{placa}/combustible")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActualizarCombustible(string placa, [FromBody] decimal nivelCombustible)
        {
            try
            {
                var result = await _vehiculoService.ActualizarCombustibleByPlacaAsync(placa, nivelCombustible);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Vehículo con placa {placa} no encontrado"));

                return Ok(ApiResponse<bool>.Ok(true, "Nivel de combustible actualizado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
}