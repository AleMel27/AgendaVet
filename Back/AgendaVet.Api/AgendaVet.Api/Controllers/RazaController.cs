using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RazaController : ControllerBase
    {
        private readonly IRazaService _razaService;

        public RazaController(IRazaService razaService)
        {
            _razaService = razaService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var razas = await _razaService.ObtenerTodasAsync();
            return Ok(razas);
        }

        [HttpGet("especie/{idEspecie}")]
        public async Task<IActionResult> ObtenerPorEspecie(int idEspecie)
        {
            var razas = await _razaService.ObtenerPorEspecieAsync(idEspecie);
            return Ok(razas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var raza = await _razaService.ObtenerPorIdAsync(id);
            if (raza == null) return NotFound(new { mensaje = "Raza no encontrada." });
            return Ok(raza);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] RazaReqDto dto)
        {
            var creada = await _razaService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.IdRaza }, creada);
        }
    }
}