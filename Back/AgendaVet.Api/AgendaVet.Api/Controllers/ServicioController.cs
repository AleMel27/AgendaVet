using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServicioController : ControllerBase
    {
        private readonly IServicioService _servicioService;

        public ServicioController(IServicioService servicioService)
        {
            _servicioService = servicioService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos([FromQuery] bool soloActivos = true)
        {
            var servicios = await _servicioService.ObtenerTodosAsync(soloActivos);
            return Ok(servicios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var servicio = await _servicioService.ObtenerPorIdAsync(id);
            if (servicio == null) return NotFound(new { mensaje = "Servicio no encontrado." });
            return Ok(servicio);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ServicioReqDto dto)
        {
            var creado = await _servicioService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.IdServicio }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ServicioReqDto dto)
        {
            var actualizado = await _servicioService.ActualizarAsync(id, dto);
            if (actualizado == null) return NotFound(new { mensaje = "Servicio no encontrado para actualizar." });
            return Ok(actualizado);
        }
    }
}