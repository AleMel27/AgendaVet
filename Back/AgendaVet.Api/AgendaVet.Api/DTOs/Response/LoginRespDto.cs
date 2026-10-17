namespace AgendaVet.Api.DTOs.Response
{
    public class LoginRespDto
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioRespDto Usuario { get; set; } = null!;
    }
}
