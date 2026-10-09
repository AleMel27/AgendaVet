namespace AgendaVet.Api.Models
{
    public class Paciente
    {
        public int IdPaciente { get; set; }
        public int IdPropietario { get; set; }
        public int IdEspecie { get; set; }
        public int? IdRaza { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty; // 'Macho' | 'Hembra'
        public DateTime? FechaNacimiento { get; set; }
        public string? Color { get; set; }
        public bool Esterilizado { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}