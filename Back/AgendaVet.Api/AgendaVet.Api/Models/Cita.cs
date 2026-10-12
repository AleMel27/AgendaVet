using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaVet.Api.Models
{
    [Table("citas")]
    public class Cita
    {
        [Key]
        [Column("id_cita")]
        public int IdCita { get; set; }

        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Column("id_veterinario")]
        public int IdVeterinario { get; set; }

        [Column("id_servicio")]
        public int IdServicio { get; set; }

        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = null!;

        [Column("motivo")]
        public string? Motivo { get; set; }

        [Column("notas_agendamiento")]
        public string? NotasAgendamiento { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}