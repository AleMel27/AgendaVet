using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CitaController : ControllerBase
    {
        private readonly ICitaService _citaService;

        public CitaController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPaginado([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? idVeterinario = null, [FromQuery] string? estado = null)
        {
            var resultado = await _citaService.ObtenerPaginadoAsync(page, pageSize, idVeterinario, estado);
            return Ok(resultado);
        }

        [HttpGet("todas")]
        public async Task<IActionResult> ObtenerTodas()
        {
            var citas = await _citaService.ObtenerTodasAsync();
            return Ok(citas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var cita = await _citaService.ObtenerPorIdAsync(id);
            if (cita == null) return NotFound(new { mensaje = "Cita no encontrada." });
            return Ok(cita);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CitaReqDto dto)
        {
            var creada = await _citaService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.IdCita }, creada);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] CitaUpdateReqDto dto)
        {
            var actualizada = await _citaService.ActualizarAsync(id, dto);
            if (actualizada == null) return NotFound(new { mensaje = "Cita no encontrada para actualizar." });
            return Ok(actualizada);
        }

        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromQuery] string nuevoEstado)
        {
            var cambiado = await _citaService.CambiarEstadoAsync(id, nuevoEstado);
            if (!cambiado) return NotFound(new { mensaje = "Cita no encontrada." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _citaService.EliminarAsync(id);
            if (!eliminado) return NotFound(new { mensaje = "Cita no encontrada." });
            return NoContent();
        }
    }
}