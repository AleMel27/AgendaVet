namespace AgendaVet.Api.DTOs.Response
{
    public class RolRespDto
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
}
