namespace AgendaVet.Api.DTOs.Response
{
    public class PacienteRespDto
    {
        public int IdPaciente { get; set; }
        public int IdPropietario { get; set; }
        public string PropietarioNombreCompleto { get; set; } = string.Empty;
        public int IdEspecie { get; set; }
        public string EspecieNombre { get; set; } = string.Empty;
        public int? IdRaza { get; set; }
        public string? RazaNombre { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public string? Color { get; set; }
        public bool Esterilizado { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
