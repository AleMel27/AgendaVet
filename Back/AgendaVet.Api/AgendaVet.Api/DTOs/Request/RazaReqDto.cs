using System.ComponentModel.DataAnnotations;

namespace AgendaVet.Api.DTOs.Request
{
    public class RazaReqDto
    {
        [Required(ErrorMessage = "La especie es obligatoria.")]
        public int IdEspecie { get; set; }

        [Required(ErrorMessage = "El nombre de la raza es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
    }
}