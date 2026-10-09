using AgendaVet.Api.Models;

namespace AgendaVet.Api.Interfaces
{
    public interface IJwtService
    {
        string GenerarToken(Usuario usuario, string rolNombre);
    }
}