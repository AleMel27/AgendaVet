using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaVet.Api.Models
{
    [Table("pacientes")]
    public class Paciente
    {
        [Key]
        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Column("id_propietario")]
        public int IdPropietario { get; set; }

        [Column("id_especie")]
        public int IdEspecie { get; set; }

        [Column("id_raza")]
        public int? IdRaza { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("sexo")]
        public string Sexo { get; set; } = string.Empty; // 'Macho' | 'Hembra'

        [Column("fecha_nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Column("color")]
        public string? Color { get; set; }

        [Column("esterilizado")]
        public bool Esterilizado { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }
    }
}