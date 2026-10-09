using AgendaVet.Api.DTOs.Response;

namespace AgendaVet.Api.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<RolRespDto>> ObtenerTodosAsync();
        Task<RolRespDto?> ObtenerPorIdAsync(int id);
    }
}