using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPaginado([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? buscar = null)
        {
            var resultado = await _usuarioService.ObtenerPaginadoAsync(page, pageSize, buscar);
            return Ok(resultado);
        }

        [HttpGet("todos")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var usuario = await _usuarioService.ObtenerPorIdAsync(id);
            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado." });
            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] UsuarioReqDto dto)
        {
            var creado = await _usuarioService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.IdUsuario }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] UsuarioUpdateReqDto dto)
        {
            var actualizado = await _usuarioService.ActualizarAsync(id, dto);
            if (actualizado == null) return NotFound(new { mensaje = "Usuario no encontrado para actualizar." });
            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _usuarioService.EliminarAsync(id);
            if (!eliminado) return NotFound(new { mensaje = "Usuario no encontrado." });
            return NoContent();
        }
    }
}