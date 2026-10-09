namespace AgendaVet.Api.Models
{
    public class Cita
    {
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }
        public int IdVeterinario { get; set; }
        public int IdServicio { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; } = "Pendiente"; // 'Pendiente', 'Confirmada', 'En Atencion', 'Completada', 'Cancelada'
        public string? Motivo { get; set; }
        public string? NotasAgendamiento { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}