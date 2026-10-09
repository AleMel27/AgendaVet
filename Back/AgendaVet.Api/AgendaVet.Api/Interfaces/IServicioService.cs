using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;

namespace AgendaVet.Api.Interfaces
{
    public interface IServicioService
    {
        Task<IEnumerable<ServicioRespDto>> ObtenerTodosAsync(bool soloActivos = true);
        Task<ServicioRespDto?> ObtenerPorIdAsync(int id);
        Task<ServicioRespDto> CrearAsync(ServicioReqDto dto);
        Task<ServicioRespDto?> ActualizarAsync(int id, ServicioReqDto dto);
    }
}