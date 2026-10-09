namespace AgendaVet.Api.DTOs.Response
{
    public class PropietarioRespDto
    {
        public int IdPropietario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string DniCe { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
