using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PropietarioController : ControllerBase
    {
        private readonly IPropietarioService _propietarioService;

        public PropietarioController(IPropietarioService propietarioService)
        {
            _propietarioService = propietarioService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPaginado([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? buscar = null)
        {
            var resultado = await _propietarioService.ObtenerPaginadoAsync(page, pageSize, buscar);
            return Ok(resultado);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var propietarios = await _propietarioService.ObtenerTodosAsync();
            return Ok(propietarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var propietario = await _propietarioService.ObtenerPorIdAsync(id);
            if (propietario == null) return NotFound(new { mensaje = "Propietario no encontrado." });
            return Ok(propietario);
        }

        [HttpGet("dni/{dniCe}")]
        public async Task<IActionResult> ObtenerPorDni(string dniCe)
        {
            var propietario = await _propietarioService.ObtenerPorDniAsync(dniCe);
            if (propietario == null) return NotFound(new { mensaje = "Propietario con ese DNI/CE no encontrado." });
            return Ok(propietario);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PropietarioReqDto dto)
        {
            var creado = await _propietarioService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.IdPropietario }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] PropietarioReqDto dto)
        {
            var actualizado = await _propietarioService.ActualizarAsync(id, dto);
            if (actualizado == null) return NotFound(new { mensaje = "Propietario no encontrado para actualizar." });
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _propietarioService.EliminarAsync(id);
            if (!eliminado) return NotFound(new { mensaje = "Propietario no encontrado." });
            return NoContent();
        }
    }
}