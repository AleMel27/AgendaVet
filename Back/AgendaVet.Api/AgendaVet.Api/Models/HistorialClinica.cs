using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaVet.Api.Models
{
    [Table("historias_clinicas")]
    public class HistoriaClinica
    {
        [Key]
        [Column("id_historia")]
        public int IdHistoria { get; set; }

        [Column("id_cita")]
        public int? IdCita { get; set; }

        [Column("id_paciente")]
        public int IdPaciente { get; set; }

        [Column("id_veterinario")]
        public int IdVeterinario { get; set; }

        [Column("fecha_atencion")]
        public DateTime FechaAtencion { get; set; }

        [Column("peso_kg")]
        public decimal? PesoKg { get; set; }

        [Column("temperatura")]
        public decimal? Temperatura { get; set; }

        [Column("frecuencia_cardiaca")]
        public int? FrecuenciaCardiaca { get; set; }

        [Column("diagnostico")]
        public string Diagnostico { get; set; } = string.Empty;

        [Column("tratamiento")]
        public string? Tratamiento { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }
    }
}