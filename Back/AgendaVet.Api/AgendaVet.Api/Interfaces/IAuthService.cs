using AgendaVet.Api.DTOs.Request;
using AgendaVet.Api.DTOs.Response;

namespace AgendaVet.Api.Interfaces
{
    public interface IAuthService
    {
        Task<LoginRespDto?> LoginAsync(LoginReqDto dto);
    }
}