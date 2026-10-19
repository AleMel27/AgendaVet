using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;

        public PacienteController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPaginado([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? buscar = null)
        {
            var resultado = await _pacienteService.ObtenerPaginadoAsync(page, pageSize, buscar);
            return Ok(resultado);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var pacientes = await _pacienteService.ObtenerTodosAsync();
            return Ok(pacientes);
        }

        [HttpGet("propietario/{idPropietario}")]
        public async Task<IActionResult> ObtenerPorPropietario(int idPropietario)
        {
            var pacientes = await _pacienteService.ObtenerPorPropietarioAsync(idPropietario);
            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var paciente = await _pacienteService.ObtenerPorIdAsync(id);
            if (paciente == null) return NotFound(new { mensaje = "Paciente no encontrado." });
            return Ok(paciente);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PacienteReqDto dto)
        {
            var creado = await _pacienteService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.IdPaciente }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] PacienteReqDto dto)
        {
            var actualizado = await _pacienteService.ActualizarAsync(id, dto);
            if (actualizado == null) return NotFound(new { mensaje = "Paciente no encontrado para actualizar." });
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _pacienteService.EliminarAsync(id);
            if (!eliminado) return NotFound(new { mensaje = "Paciente no encontrado." });
            return NoContent();
        }
    }
}