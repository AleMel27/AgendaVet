using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;

namespace AgendaVet.Api.Interfaces
{
    public interface IPacienteService
    {
        Task<IEnumerable<PacienteRespDto>> ObtenerTodosAsync();
        Task<PaginatedRespDto> ObtenerPaginadoAsync(int pageNumber, int pageSize, string? buscar = null);
        Task<PacienteRespDto?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<PacienteRespDto>> ObtenerPorPropietarioAsync(int idPropietario);
        Task<PacienteRespDto> CrearAsync(PacienteReqDto dto);
        Task<PacienteRespDto?> ActualizarAsync(int id, PacienteReqDto dto);
        Task<bool> EliminarAsync(int id);
    }
}