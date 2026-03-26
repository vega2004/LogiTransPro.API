using LogiTransPro.API.Attributes;
using LogiTransPro.API.Models.DTOs.OrdenCarga;
using LogiTransPro.API.Models.ViewModels;
using LogiTransPro.API.Services.OrdenCarga;
using Microsoft.AspNetCore.Mvc;

namespace LogiTransPro.API.Controllers
{
    [ApiController]
    [Route("api/ordenes-carga")]
    [AuthorizeRole]
    public class OrdenesCargaController : ControllerBase
    {
        private readonly IOrdenCargaService _ordenCargaService;
        private readonly ILogger<OrdenesCargaController> _logger;

        public OrdenesCargaController(IOrdenCargaService ordenCargaService, ILogger<OrdenesCargaController> logger)
        {
            _ordenCargaService = ordenCargaService;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        /// <summary>
        /// GET /api/ordenes-carga - Obtener todas las órdenes de carga
        /// </summary>
        [HttpGet]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<OrdenCargaDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var ordenes = await _ordenCargaService.GetAllAsync();
            return Ok(ApiResponse<List<OrdenCargaDTO>>.Ok(ordenes));
        }

        /// <summary>
        /// GET /api/ordenes-carga/pendientes - Obtener órdenes pendientes
        /// </summary>
        [HttpGet("pendientes")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<OrdenCargaDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendientes()
        {
            var ordenes = await _ordenCargaService.GetPendientesAsync();
            return Ok(ApiResponse<List<OrdenCargaDTO>>.Ok(ordenes));
        }

        /// <summary>
        /// GET /api/ordenes-carga/urgentes - Obtener órdenes urgentes
        /// </summary>
        [HttpGet("urgentes")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<OrdenCargaDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUrgentes()
        {
            var ordenes = await _ordenCargaService.GetUrgentesAsync();
            return Ok(ApiResponse<List<OrdenCargaDTO>>.Ok(ordenes));
        }

        // ======================================================
        // BÚSQUEDAS POR CAMPOS ÚNICOS
        // ======================================================

        /// <summary>
        /// GET /api/ordenes-carga/numero/{numeroOrden} - Obtener orden por número de orden
        /// </summary>
        [HttpGet("numero/{numeroOrden}")]
        [ProducesResponseType(typeof(ApiResponse<OrdenCargaDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNumeroOrden(string numeroOrden)
        {
            var orden = await _ordenCargaService.GetByNumeroOrdenAsync(numeroOrden);
            if (orden == null)
                return NotFound(ApiResponse<object>.Error($"Orden de carga {numeroOrden} no encontrada"));

            return Ok(ApiResponse<OrdenCargaDTO>.Ok(orden));
        }

        /// <summary>
        /// GET /api/ordenes-carga/cliente/rfc/{rfc} - Obtener órdenes por RFC del cliente
        /// </summary>
        [HttpGet("cliente/rfc/{rfc}")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<List<OrdenCargaDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByClienteRfc(string rfc)
        {
            var ordenes = await _ordenCargaService.GetByClienteRfcAsync(rfc);
            return Ok(ApiResponse<List<OrdenCargaDTO>>.Ok(ordenes));
        }

        // ======================================================
        // CRUD USANDO NÚMERO DE ORDEN
        // ======================================================

        /// <summary>
        /// POST /api/ordenes-carga - Crear nueva orden de carga
        /// </summary>
        [HttpPost]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<OrdenCargaDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CrearOrdenCargaDTO createDto)
        {
            try
            {
                var orden = await _ordenCargaService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetByNumeroOrden), new { numeroOrden = orden.NumeroOrden },
                    ApiResponse<OrdenCargaDTO>.Ok(orden, "Orden de carga creada exitosamente"));
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
        /// PUT /api/ordenes-carga/numero/{numeroOrden} - Actualizar orden por número (Solo Admin y Supervisor)
        /// </summary>
        [HttpPut("numero/{numeroOrden}")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateByNumeroOrden(string numeroOrden, [FromBody] OrdenCargaDTO updateDto)
        {
            try
            {
                var result = await _ordenCargaService.UpdateByNumeroOrdenAsync(numeroOrden, updateDto);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Orden de carga {numeroOrden} no encontrada"));

                return Ok(ApiResponse<bool>.Ok(true, "Orden de carga actualizada exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// PATCH /api/ordenes-carga/numero/{numeroOrden}/cancelar - Cancelar orden por número (Solo Admin)
        /// </summary>
        [HttpPatch("numero/{numeroOrden}/cancelar")]
        [AdminOnly]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelarByNumeroOrden(string numeroOrden, [FromBody] CancelarOrdenDTO cancelarDto)
        {
            try
            {
                var result = await _ordenCargaService.CancelarByNumeroOrdenAsync(numeroOrden, cancelarDto.Motivo);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Orden de carga {numeroOrden} no encontrada"));

                return Ok(ApiResponse<bool>.Ok(true, "Orden de carga cancelada exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// PATCH /api/ordenes-carga/numero/{numeroOrden}/asignar-viaje/{numeroViaje} - Asignar viaje a orden
        /// </summary>
        [HttpPatch("numero/{numeroOrden}/asignar-viaje/{numeroViaje}")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AsignarViaje(string numeroOrden, string numeroViaje)
        {
            var result = await _ordenCargaService.AsignarViajeByNumeroOrdenAsync(numeroOrden, numeroViaje);
            if (!result)
                return NotFound(ApiResponse<object>.Error($"Orden {numeroOrden} o viaje {numeroViaje} no encontrado"));

            return Ok(ApiResponse<bool>.Ok(true, "Viaje asignado exitosamente"));
        }
    }

    public class CancelarOrdenDTO
    {
        public string Motivo { get; set; } = string.Empty;
    }
}