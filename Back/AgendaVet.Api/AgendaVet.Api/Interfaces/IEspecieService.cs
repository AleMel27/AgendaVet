using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;

namespace AgendaVet.Api.Interfaces
{
    public interface IEspecieService
    {
        Task<IEnumerable<EspecieRespDto>> ObtenerTodasAsync();
        Task<EspecieRespDto?> ObtenerPorIdAsync(int id);
        Task<EspecieRespDto> CrearAsync(EspecieReqDto dto);
    }
}