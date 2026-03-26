using LogiTransPro.API.Attributes;
using LogiTransPro.API.Models.DTOs.Ruta;
using LogiTransPro.API.Models.ViewModels;
using LogiTransPro.API.Services.Ruta;
using Microsoft.AspNetCore.Mvc;

namespace LogiTransPro.API.Controllers
{
    [ApiController]
    [Route("api/rutas")]
    [AuthorizeRole]
    public class RutasController : ControllerBase
    {
        private readonly IRutaService _rutaService;
        private readonly ILogger<RutasController> _logger;

        public RutasController(IRutaService rutaService, ILogger<RutasController> logger)
        {
            _rutaService = rutaService;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        /// <summary>
        /// GET /api/rutas - Obtener todas las rutas
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<RutaDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var rutas = await _rutaService.GetAllAsync();
            return Ok(ApiResponse<List<RutaDTO>>.Ok(rutas));
        }

        /// <summary>
        /// GET /api/rutas/activas - Obtener rutas activas
        /// </summary>
        [HttpGet("activas")]
        [ProducesResponseType(typeof(ApiResponse<List<RutaDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivas()
        {
            var rutas = await _rutaService.GetActivasAsync();
            return Ok(ApiResponse<List<RutaDTO>>.Ok(rutas));
        }

        // ======================================================
        // BÚSQUEDA POR CÓDIGO DE RUTA
        // ======================================================

        /// <summary>
        /// GET /api/rutas/codigo/{codigo} - Obtener ruta por código
        /// </summary>
        [HttpGet("codigo/{codigo}")]
        [ProducesResponseType(typeof(ApiResponse<RutaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCodigo(string codigo)
        {
            var ruta = await _rutaService.GetByCodigoAsync(codigo);
            if (ruta == null)
                return NotFound(ApiResponse<object>.Error($"Ruta con código {codigo} no encontrada"));

            return Ok(ApiResponse<RutaDTO>.Ok(ruta));
        }

        // ======================================================
        // BÚSQUEDA POR ORIGEN Y DESTINO
        // ======================================================

        /// <summary>
        /// GET /api/rutas/buscar?origen={origen}&destino={destino} - Buscar rutas por origen y destino
        /// </summary>
        [HttpGet("buscar")]
        [ProducesResponseType(typeof(ApiResponse<List<RutaDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Buscar([FromQuery] string origen, [FromQuery] string destino)
        {
            var rutas = await _rutaService.GetByOrigenDestinoAsync(origen, destino);
            return Ok(ApiResponse<List<RutaDTO>>.Ok(rutas));
        }

        // ======================================================
        // OPTIMIZACIÓN DE RUTA
        // ======================================================

        /// <summary>
        /// POST /api/rutas/optimizar - Optimizar ruta
        /// </summary>
        [HttpPost("optimizar")]
        [ProducesResponseType(typeof(ApiResponse<RutaOptimizadaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OptimizarRuta([FromBody] OptimizarRutaDTO optimizarDto)
        {
            var rutaOptimizada = await _rutaService.OptimizarRutaAsync(optimizarDto);
            return Ok(ApiResponse<RutaOptimizadaDTO>.Ok(rutaOptimizada, "Ruta optimizada calculada"));
        }

        // ======================================================
        // CRUD USANDO CÓDIGO DE RUTA
        // ======================================================

        /// <summary>
        /// POST /api/rutas - Crear nueva ruta (Solo Admin)
        /// </summary>
        [HttpPost]
        [AdminOnly]
        [ProducesResponseType(typeof(ApiResponse<RutaDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RutaDTO createDto)
        {
            try
            {
                var ruta = await _rutaService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetByCodigo), new { codigo = ruta.CodigoRuta },
                    ApiResponse<RutaDTO>.Ok(ruta, "Ruta creada exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// PUT /api/rutas/codigo/{codigo} - Actualizar ruta por código (Solo Admin)
        /// </summary>
        [HttpPut("codigo/{codigo}")]
        [AdminOnly]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateByCodigo(string codigo, [FromBody] RutaDTO updateDto)
        {
            try
            {
                var result = await _rutaService.UpdateByCodigoAsync(codigo, updateDto);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Ruta con código {codigo} no encontrada"));

                return Ok(ApiResponse<bool>.Ok(true, "Ruta actualizada exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// DELETE /api/rutas/codigo/{codigo} - Eliminar ruta por código (Solo Admin)
        /// </summary>
        [HttpDelete("codigo/{codigo}")]
        [AdminOnly]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteByCodigo(string codigo)
        {
            try
            {
                var result = await _rutaService.DeleteByCodigoAsync(codigo);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Ruta con código {codigo} no encontrada"));

                return Ok(ApiResponse<bool>.Ok(true, "Ruta eliminada exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
}