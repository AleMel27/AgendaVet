using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaVet.Api.Models
{
    [Table("razas")]
    public class Raza
    {
        [Key]
        [Column("id_raza")]
        public int IdRaza { get; set; }

        [Column("id_especie")]
        public int IdEspecie { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}