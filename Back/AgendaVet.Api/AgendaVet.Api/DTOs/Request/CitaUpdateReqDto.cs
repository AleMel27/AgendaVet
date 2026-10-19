using System.ComponentModel.DataAnnotations;

namespace AgendaVet.Api.DTOs.Request
{
    public class CitaUpdateReqDto
    {
        [Required(ErrorMessage = "El veterinario es obligatorio.")]
        public int IdVeterinario { get; set; }

        [Required(ErrorMessage = "El servicio es obligatorio.")]
        public int IdServicio { get; set; }

        [Required(ErrorMessage = "La fecha y hora son obligatorias.")]
        public DateTime FechaHora { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [StringLength(30, ErrorMessage = "El estado no puede superar los 30 caracteres.")]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(255, ErrorMessage = "El motivo no puede superar los 255 caracteres.")]
        public string? Motivo { get; set; }

        [StringLength(500, ErrorMessage = "Las notas no pueden superar los 500 caracteres.")]
        public string? NotasAgendamiento { get; set; }
    }
}
