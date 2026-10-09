namespace AgendaVet.Api.Models
{
    public class HistoriaClinica
    {
        public int IdHistoria { get; set; }
        public int? IdCita { get; set; }
        public int IdPaciente { get; set; }
        public int IdVeterinario { get; set; }
        public DateTime FechaAtencion { get; set; }
        public decimal? PesoKg { get; set; }
        public decimal? Temperatura { get; set; }
        public int? FrecuenciaCardiaca { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public string? Tratamiento { get; set; }
        public string? Observaciones { get; set; }
    }
}