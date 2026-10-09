using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EspecieController : ControllerBase
    {
        private readonly IEspecieService _especieService;

        public EspecieController(IEspecieService especieService)
        {
            _especieService = especieService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var especies = await _especieService.ObtenerTodasAsync();
            return Ok(especies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var especie = await _especieService.ObtenerPorIdAsync(id);
            if (especie == null) return NotFound(new { mensaje = "Especie no encontrada." });
            return Ok(especie);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] EspecieReqDto dto)
        {
            var creada = await _especieService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.IdEspecie }, creada);
        }
    }
}