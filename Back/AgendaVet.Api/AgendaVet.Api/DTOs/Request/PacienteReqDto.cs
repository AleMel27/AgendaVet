using System.ComponentModel.DataAnnotations;

namespace AgendaVet.Api.DTOs.Request
{
    public class PacienteReqDto
    {
        [Required(ErrorMessage = "El propietario es obligatorio.")]
        public int IdPropietario { get; set; }

        [Required(ErrorMessage = "La especie es obligatoria.")]
        public int IdEspecie { get; set; }

        public int? IdRaza { get; set; }

        [Required(ErrorMessage = "El nombre del paciente es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El sexo es obligatorio.")]
        [RegularExpression("^(Macho|Hembra)$", ErrorMessage = "El sexo debe ser 'Macho' o 'Hembra'.")]
        public string Sexo { get; set; } = string.Empty;

        public DateTime? FechaNacimiento { get; set; }

        [StringLength(30, ErrorMessage = "El color no puede superar los 30 caracteres.")]
        public string? Color { get; set; }

        public bool Esterilizado { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
    }
}