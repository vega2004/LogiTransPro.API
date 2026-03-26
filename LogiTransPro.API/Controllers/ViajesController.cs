using LogiTransPro.API.Attributes;
using LogiTransPro.API.Models.DTOs.Viaje;
using LogiTransPro.API.Models.ViewModels;
using LogiTransPro.API.Services.Viaje;
using Microsoft.AspNetCore.Mvc;

namespace LogiTransPro.API.Controllers
{
    [ApiController]
    [Route("api/viajes")]
    [AuthorizeRole]
    public class ViajesController : ControllerBase
    {
        private readonly IViajeService _viajeService;
        private readonly ILogger<ViajesController> _logger;

        public ViajesController(IViajeService viajeService, ILogger<ViajesController> logger)
        {
            _viajeService = viajeService;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        /// <summary>
        /// GET /api/viajes - Obtener todos los viajes
        /// </summary>
        [HttpGet]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<ViajeDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var viajes = await _viajeService.GetAllAsync();
            return Ok(ApiResponse<List<ViajeDTO>>.Ok(viajes));
        }

        /// <summary>
        /// GET /api/viajes/activos - Obtener viajes activos (en ruta)
        /// </summary>
        [HttpGet("activos")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<ViajeDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivos()
        {
            var viajes = await _viajeService.GetActivosAsync();
            return Ok(ApiResponse<List<ViajeDTO>>.Ok(viajes));
        }

        /// <summary>
        /// GET /api/viajes/proximos - Obtener viajes próximos
        /// </summary>
        [HttpGet("proximos")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<ViajeDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProximos([FromQuery] int dias = 7)
        {
            var viajes = await _viajeService.GetProximosAsync(dias);
            return Ok(ApiResponse<List<ViajeDTO>>.Ok(viajes));
        }

        // ======================================================
        // BÚSQUEDAS POR CAMPOS ÚNICOS
        // ======================================================

        /// <summary>
        /// GET /api/viajes/numero/{numeroViaje} - Obtener viaje por número de viaje
        /// </summary>
        [HttpGet("numero/{numeroViaje}")]
        [ProducesResponseType(typeof(ApiResponse<ViajeDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNumeroViaje(string numeroViaje)
        {
            var viaje = await _viajeService.GetByNumeroViajeAsync(numeroViaje);
            if (viaje == null)
                return NotFound(ApiResponse<object>.Error($"Viaje {numeroViaje} no encontrado"));

            return Ok(ApiResponse<ViajeDTO>.Ok(viaje));
        }

        /// <summary>
        /// GET /api/viajes/vehiculo/placa/{placa} - Obtener viajes por placa del vehículo
        /// </summary>
        [HttpGet("vehiculo/placa/{placa}")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<ViajeDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByPlacaVehiculo(string placa)
        {
            try
            {
                var viajes = await _viajeService.GetByPlacaVehiculoAsync(placa);
                return Ok(ApiResponse<List<ViajeDTO>>.Ok(viajes));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// GET /api/viajes/chofer/email/{email} - Obtener viajes por email del chofer
        /// </summary>
        [HttpGet("chofer/email/{email}")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<ViajeDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEmailChofer(string email)
        {
            try
            {
                var viajes = await _viajeService.GetByEmailChoferAsync(email);
                return Ok(ApiResponse<List<ViajeDTO>>.Ok(viajes));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// GET /api/viajes/mis-viajes - Obtener mis viajes (chofer autenticado)
        /// </summary>
        [HttpGet("mis-viajes")]
        [ChoferOnly]
        [ProducesResponseType(typeof(ApiResponse<List<ViajeDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMisViajes()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized(ApiResponse<object>.Error("No se pudo identificar al usuario"));

            var viajes = await _viajeService.GetByEmailChoferAsync(email);
            return Ok(ApiResponse<List<ViajeDTO>>.Ok(viajes));
        }

        // ======================================================
        // CRUD USANDO NÚMERO DE VIAJE
        // ======================================================

        /// <summary>
        /// POST /api/viajes - Crear nuevo viaje (Solo Admin y Supervisor)
        /// </summary>
        [HttpPost]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<ViajeDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CrearViajeDTO createDto)
        {
            try
            {
                var viaje = await _viajeService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetByNumeroViaje), new { numeroViaje = viaje.NumeroViaje },
                    ApiResponse<ViajeDTO>.Ok(viaje, "Viaje creado exitosamente"));
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
        /// POST /api/viajes/numero/{numeroViaje}/iniciar - Iniciar viaje (Solo chofer asignado)
        /// </summary>
        [HttpPost("numero/{numeroViaje}/iniciar")]
        [ChoferOnly]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> IniciarViaje(string numeroViaje, [FromBody] IniciarViajeDTO iniciarDto)
        {
            try
            {
                var result = await _viajeService.IniciarViajeByNumeroAsync(numeroViaje, iniciarDto);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Viaje {numeroViaje} no encontrado"));

                return Ok(ApiResponse<bool>.Ok(true, "Viaje iniciado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// POST /api/viajes/numero/{numeroViaje}/finalizar - Finalizar viaje (Solo chofer asignado)
        /// </summary>
        [HttpPost("numero/{numeroViaje}/finalizar")]
        [ChoferOnly]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FinalizarViaje(string numeroViaje, [FromBody] FinalizarViajeDTO finalizarDto)
        {
            try
            {
                var result = await _viajeService.FinalizarViajeByNumeroAsync(numeroViaje, finalizarDto);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Viaje {numeroViaje} no encontrado"));

                return Ok(ApiResponse<bool>.Ok(true, "Viaje finalizado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// PATCH /api/viajes/numero/{numeroViaje}/cancelar - Cancelar viaje (Solo Admin)
        /// </summary>
        [HttpPatch("numero/{numeroViaje}/cancelar")]
        [AdminOnly]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelarViaje(string numeroViaje, [FromBody] CancelarViajeDTO cancelarDto)
        {
            try
            {
                var result = await _viajeService.CancelarViajeByNumeroAsync(numeroViaje, cancelarDto.Motivo);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Viaje {numeroViaje} no encontrado"));

                return Ok(ApiResponse<bool>.Ok(true, "Viaje cancelado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }

    public class CancelarViajeDTO
    {
        public string Motivo { get; set; } = string.Empty;
    }
}