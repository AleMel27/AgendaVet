using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgendaVet.Api.Models
{
    [Table("servicios")]
    public class Servicio
    {
        [Key]
        [Column("id_servicio")]
        public int IdServicio { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("duracion_estimada_min")]
        public int DuracionEstimadaMin { get; set; } = 30;

        [Column("activo")]
        public bool Activo { get; set; } = true;
    }
}