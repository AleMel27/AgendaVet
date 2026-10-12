using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaVet.Api.Models
{
    [Table("especies")]
    public class Especie
    {
        [Key]
        [Column("id_especie")]
        public int IdEspecie { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}