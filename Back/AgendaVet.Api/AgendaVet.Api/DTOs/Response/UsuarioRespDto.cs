namespace AgendaVet.Api.DTOs.Response
{
    public class UsuarioRespDto
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string RolNombre { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
