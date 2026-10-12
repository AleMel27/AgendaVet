using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaVet.Api.Models
{
    [Table("propietarios")]
    public class Propietario
    {
        [Key]
        [Column("id_propietario")]
        public int IdPropietario { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Column("dni_ce")]
        public string DniCe { get; set; } = string.Empty;

        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [Column("email")]
        public string? Email { get; set; }

        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; }
    }
}