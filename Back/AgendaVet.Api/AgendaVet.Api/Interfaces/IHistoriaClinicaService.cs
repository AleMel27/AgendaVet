using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;

namespace AgendaVet.Api.Interfaces
{
    public interface IHistoriaClinicaService
    {
        Task<IEnumerable<HistoriaClinicaRespDto>> ObtenerPorPacienteAsync(int idPaciente);
        Task<PaginatedRespDto> ObtenerPaginadoPorPacienteAsync(int idPaciente, int pageNumber, int pageSize);
        Task<HistoriaClinicaRespDto?> ObtenerPorIdAsync(int id);
        Task<HistoriaClinicaRespDto> CrearAsync(HistoriaClinicaReqDto dto);
    }
}