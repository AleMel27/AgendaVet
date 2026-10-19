using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;

namespace AgendaVet.Api.Interfaces
{
    public interface IRazaService
    {
        Task<IEnumerable<RazaRespDto>> ObtenerTodasAsync();
        Task<IEnumerable<RazaRespDto>> ObtenerPorEspecieAsync(int idEspecie);
        Task<RazaRespDto?> ObtenerPorIdAsync(int id);
        Task<RazaRespDto> CrearAsync(RazaReqDto dto);
    }
}