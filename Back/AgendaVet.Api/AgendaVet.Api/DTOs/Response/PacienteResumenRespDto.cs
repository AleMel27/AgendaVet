namespace AgendaVet.Api.DTOs.Response
{
    public class PacienteResumenRespDto
    {
        public int IdPaciente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string EspecieNombre { get; set; } = string.Empty;
    }
}
