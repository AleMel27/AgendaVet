using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;

namespace AgendaVet.Api.Interfaces
{
    public interface ICitaService
    {
        Task<IEnumerable<CitaRespDto>> ObtenerTodasAsync();
        Task<PaginatedRespDto> ObtenerPaginadoAsync(int pageNumber, int pageSize, int? idVeterinario = null, string? estado = null);
        Task<CitaRespDto?> ObtenerPorIdAsync(int id);
        Task<CitaRespDto> CrearAsync(CitaReqDto dto);
        Task<CitaRespDto?> ActualizarAsync(int id, CitaUpdateReqDto dto);
        Task<bool> CambiarEstadoAsync(int id, string nuevoEstado);
        Task<bool> EliminarAsync(int id);
    }
}