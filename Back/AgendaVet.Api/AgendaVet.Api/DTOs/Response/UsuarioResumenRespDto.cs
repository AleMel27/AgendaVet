namespace AgendaVet.Api.DTOs.Response
{
    public class UsuarioResumenRespDto
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
