namespace AgendaVet.Api.DTOs.Response
{
    public class PropietarioResumenRespDto
    {
        public int IdPropietario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string DniCe { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }
}
