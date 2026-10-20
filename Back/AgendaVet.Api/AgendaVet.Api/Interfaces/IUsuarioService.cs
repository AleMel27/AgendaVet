using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;

namespace AgendaVet.Api.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioRespDto>> ObtenerTodosAsync();
        Task<PaginatedRespDto> ObtenerPaginadoAsync(int pageNumber, int pageSize, string? buscar = null, int? idRol = null);
        Task<UsuarioRespDto?> ObtenerPorIdAsync(int id);
        Task<UsuarioRespDto> CrearAsync(UsuarioReqDto dto);
        Task<UsuarioRespDto?> ActualizarAsync(int id, UsuarioUpdateReqDto dto);
        Task<bool> EliminarAsync(int id);
    }
}