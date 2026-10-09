using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;

namespace AgendaVet.Api.Interfaces
{
    public interface IPropietarioService
    {
        Task<IEnumerable<PropietarioRespDto>> ObtenerTodosAsync();
        Task<PaginatedRespDto> ObtenerPaginadoAsync(int pageNumber, int pageSize, string? buscar = null);
        Task<PropietarioRespDto?> ObtenerPorIdAsync(int id);
        Task<PropietarioRespDto?> ObtenerPorDniAsync(string dniCe);
        Task<PropietarioRespDto> CrearAsync(PropietarioReqDto dto);
        Task<PropietarioRespDto?> ActualizarAsync(int id, PropietarioReqDto dto);
        Task<bool> EliminarAsync(int id);
    }
}