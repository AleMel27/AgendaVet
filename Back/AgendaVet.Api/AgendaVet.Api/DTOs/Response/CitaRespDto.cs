namespace AgendaVet.Api.DTOs.Response
{
    public class CitaRespDto
    {
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public int IdVeterinario { get; set; }
        public string VeterinarioNombre { get; set; } = string.Empty;
        public int IdServicio { get; set; }
        public string ServicioNombre { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Motivo { get; set; }
        public string? NotasAgendamiento { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}