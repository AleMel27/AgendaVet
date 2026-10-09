using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgendaVet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HistoriaClinicaController : ControllerBase
    {
        private readonly IHistoriaClinicaService _historiaClinicaService;

        public HistoriaClinicaController(IHistoriaClinicaService historiaClinicaService)
        {
            _historiaClinicaService = historiaClinicaService;
        }

        [HttpGet("paciente/{idPaciente}")]
        public async Task<IActionResult> ObtenerPorPaciente(int idPaciente)
        {
            var historias = await _historiaClinicaService.ObtenerPorPacienteAsync(idPaciente);
            return Ok(historias);
        }

        [HttpGet("paciente/{idPaciente}/paginado")]
        public async Task<IActionResult> ObtenerPaginadoPorPaciente(int idPaciente, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var resultado = await _historiaClinicaService.ObtenerPaginadoPorPacienteAsync(idPaciente, page, pageSize);
            return Ok(resultado);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var historia = await _historiaClinicaService.ObtenerPorIdAsync(id);
            if (historia == null) return NotFound(new { mensaje = "Historia clínica no encontrada." });
            return Ok(historia);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] HistoriaClinicaReqDto dto)
        {
            var creada = await _historiaClinicaService.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.IdHistoria }, creada);
        }
    }
}