using LogiTransPro.API.Attributes;
using LogiTransPro.API.Models.DTOs.Cliente;
using LogiTransPro.API.Models.ViewModels;
using LogiTransPro.API.Services.Cliente;
using Microsoft.AspNetCore.Mvc;

namespace LogiTransPro.API.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    [AuthorizeRole]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(IClienteService clienteService, ILogger<ClientesController> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        // ======================================================
        // LISTADOS
        // ======================================================

        /// <summary>
        /// GET /api/clientes - Obtener todos los clientes
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ClienteDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var clientes = await _clienteService.GetAllAsync();
            return Ok(ApiResponse<List<ClienteDTO>>.Ok(clientes));
        }

        /// <summary>
        /// GET /api/clientes/activos - Obtener clientes activos
        /// </summary>
        [HttpGet("activos")]
        [ProducesResponseType(typeof(ApiResponse<List<ClienteDTO>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivos()
        {
            var clientes = await _clienteService.GetActivosAsync();
            return Ok(ApiResponse<List<ClienteDTO>>.Ok(clientes));
        }

        // ======================================================
        // BÚSQUEDA POR RFC
        // ======================================================

        /// <summary>
        /// GET /api/clientes/rfc/{rfc} - Obtener cliente por RFC
        /// </summary>
        [HttpGet("rfc/{rfc}")]
        [ProducesResponseType(typeof(ApiResponse<ClienteDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByRfc(string rfc)
        {
            var cliente = await _clienteService.GetByRfcAsync(rfc);
            if (cliente == null)
                return NotFound(ApiResponse<object>.Error($"Cliente con RFC {rfc} no encontrado"));

            return Ok(ApiResponse<ClienteDTO>.Ok(cliente));
        }

        // ======================================================
        // CRUD USANDO RFC COMO IDENTIFICADOR
        // ======================================================

        /// <summary>
        /// POST /api/clientes - Crear nuevo cliente (Solo Admin y Supervisor)
        /// </summary>
        [HttpPost]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<ClienteDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CrearClienteDTO createDto)
        {
            try
            {
                var cliente = await _clienteService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetByRfc), new { rfc = cliente.Rfc },
                    ApiResponse<ClienteDTO>.Ok(cliente, "Cliente creado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// PUT /api/clientes/rfc/{rfc} - Actualizar cliente por RFC (Solo Admin y Supervisor)
        /// </summary>
        [HttpPut("rfc/{rfc}")]
        [AdminOrSupervisor]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateByRfc(string rfc, [FromBody] ClienteDTO updateDto)
        {
            try
            {
                var result = await _clienteService.UpdateByRfcAsync(rfc, updateDto);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Cliente con RFC {rfc} no encontrado"));

                return Ok(ApiResponse<bool>.Ok(true, "Cliente actualizado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }

        /// <summary>
        /// DELETE /api/clientes/rfc/{rfc} - Eliminar cliente por RFC (Soft delete - Solo Admin)
        /// </summary>
        [HttpDelete("rfc/{rfc}")]
        [AdminOnly]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteByRfc(string rfc)
        {
            try
            {
                var result = await _clienteService.DeleteByRfcAsync(rfc);
                if (!result)
                    return NotFound(ApiResponse<object>.Error($"Cliente con RFC {rfc} no encontrado"));

                return Ok(ApiResponse<bool>.Ok(true, "Cliente eliminado exitosamente"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Error(ex.Message));
            }
        }
    }
}